using API_Comidas.Data;
using API_Comidas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using Microsoft.AspNetCore.RateLimiting;

namespace API_Comidas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("general")]
    public class CouponsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CouponsController> _logger;

        public CouponsController(AppDbContext context, ILogger<CouponsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // === RESTful aliases (new) ===

        [HttpGet("")]
        public Task<ActionResult> ListCoupons() => List();

        [HttpGet("{id}")]
        public Task<ActionResult> GetCoupon(int id) => GetById(id);

        [AllowAnonymous]
        [HttpGet("available")]
        public async Task<ActionResult> GetAvailableCoupons()
        {
            try
            {
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var coupons = await _context.Coupons
                    .Include(c => c.Restaurant)
                    .Where(c => c.Active && c.Stock > 0 && c.ExpirationDate.CompareTo(today) >= 0)
                    .Select(c => new
                    {
                        c.Id,
                        c.Code,
                        c.Title,
                        c.Description,
                        c.Discount,
                        c.IsPercentage,
                        c.ExpirationDate,
                        c.Active,
                        c.Stock,
                        c.RestaurantId,
                        Restaurant = c.Restaurant != null ? new { c.Restaurant.Id, c.Restaurant.TradeName } : null
                    })
                    .ToListAsync();

                return Ok(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available coupons");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("reserved/{userId}")]
        public async Task<ActionResult> GetReservedCoupons(int userId)
        {
            // Only the user themselves or Admin
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim != "Admin" && userIdClaim != userId.ToString())
                return Forbid();

            try
            {
                var reserved = await _context.ReservedCoupons
                    .Include(rc => rc.Coupon)
                    .ThenInclude(c => c.Restaurant)
                    .Where(rc => rc.UserId == userId)
                    .Select(rc => new
                    {
                        rc.Id,
                        rc.CouponId,
                        Coupon = rc.Coupon != null ? new
                        {
                            rc.Coupon.Id,
                            rc.Coupon.Code,
                            rc.Coupon.Title,
                            rc.Coupon.Discount,
                            rc.Coupon.IsPercentage,
                            rc.Coupon.ExpirationDate,
                            Restaurant = rc.Coupon.Restaurant != null ? new { rc.Coupon.Restaurant.Id, rc.Coupon.Restaurant.TradeName } : null
                        } : null,
                        rc.ReservedAt
                    })
                    .ToListAsync();

                return Ok(reserved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reserved coupons for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetCouponsByUser(int userId)
        {
            // Only the user themselves or Admin
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim != "Admin" && userIdClaim != userId.ToString())
                return Forbid();

            try
            {
                var coupons = await _context.Coupons
                    .Include(c => c.Restaurant)
                    .Where(c => c.UserId == userId || (c.Restaurant != null && c.Restaurant.UserId == userId))
                    .Select(c => new
                    {
                        c.Id,
                        c.Code,
                        c.Title,
                        c.Description,
                        c.Discount,
                        c.IsPercentage,
                        c.ExpirationDate,
                        c.Active,
                        c.Stock,
                        c.RestaurantId,
                        Restaurant = c.Restaurant != null ? new { c.Restaurant.Id, c.Restaurant.TradeName } : null,
                        c.UserId,
                        User = c.User != null ? new { c.User.Id, c.User.Name, c.User.Email } : null
                    })
                    .ToListAsync();

                return Ok(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupons for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("{couponId}/apartar/{userId}")]
        public async Task<ActionResult> ReserveCoupon(int couponId, int userId)
        {
            // Only the user themselves or Admin
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim != "Admin" && userIdClaim != userId.ToString())
                return Forbid();

            try
            {
                var coupon = await _context.Coupons.FindAsync(couponId);
                if (coupon == null)
                    return NotFound(new { message = $"Coupon with ID {couponId} not found" });

                if (!coupon.Active || coupon.Stock <= 0)
                    return BadRequest(new { message = "Coupon is not available (inactive or out of stock)" });

                // Validate coupon hasn't expired
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                if (coupon.ExpirationDate.CompareTo(today) < 0)
                    return BadRequest(new { message = "Coupon expired" });

                // Check if already reserved by this user
                var alreadyReserved = await _context.ReservedCoupons
                    .AnyAsync(rc => rc.CouponId == couponId && rc.UserId == userId);
                if (alreadyReserved)
                    return BadRequest(new { message = "You already reserved this coupon" });

                var reserved = new ReservedCoupon
                {
                    CouponId = couponId,
                    UserId = userId,
                    ReservedAt = DateTime.UtcNow
                };

                _context.ReservedCoupons.Add(reserved);

                // Decrement stock
                coupon.Stock = coupon.Stock > 0 ? coupon.Stock - 1 : 0;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Coupon {couponId} reserved by user {userId}");
                return Ok(new { message = "Coupon reserved successfully", reserved });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving coupon {CouponId} for user {UserId}", couponId, userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpPost("")]
        public Task<ActionResult<Coupon>> CreateCoupon([FromBody] Coupon coupon) => Create(coupon);

        [Authorize(Roles = "Business,Admin")]
        [HttpPut("{id}")]
        public Task<IActionResult> UpdateCoupon(int id, [FromBody] Coupon coupon) => Update(id, coupon);

        [Authorize(Roles = "Business,Admin")]
        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteCoupon(int id) => Delete(id);

        // === Original routes (kept for backward compatibility) ===

        [HttpGet("list")]
        public async Task<ActionResult> List()
        {
            try
            {
                // IDOR: filter by ownership unless Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                IQueryable<Coupon> query = _context.Coupons
                    .Include(c => c.Restaurant)
                    .Include(c => c.User);

                // Non-Admin users only see their own coupons
                if (roleClaim != "Admin" && int.TryParse(userIdClaim, out int userId))
                {
                    query = query.Where(c =>
                        (c.Restaurant != null && c.Restaurant.UserId == userId) ||
                        (c.RestaurantId == null && c.UserId == userId));
                }

                var coupons = await query
                    .Select(c => new
                    {
                        c.Id,
                        c.Code,
                        c.Title,
                        c.Description,
                        c.Discount,
                        c.IsPercentage,
                        c.ExpirationDate,
                        c.Active,
                        c.Stock,
                        c.RestaurantId,
                        Restaurant = c.Restaurant != null ? new { c.Restaurant.Id, c.Restaurant.TradeName } : null,
                        c.UserId,
                        User = c.User != null ? new { c.User.Id, c.User.Name, c.User.Email } : null
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {coupons.Count} coupons for user {userIdClaim} (role: {roleClaim})");
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupons");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var coupon = await _context.Coupons
                    .Include(c => c.Restaurant)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (coupon == null)
                    return NotFound(new { message = $"Coupon with ID {id} not found" });

                // IDOR: verify ownership unless Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin" && int.TryParse(userIdClaim, out int userId))
                {
                    bool isOwner = (coupon.Restaurant != null && coupon.Restaurant.UserId == userId) ||
                                   (coupon.RestaurantId == null && coupon.UserId == userId);
                    if (!isOwner)
                        return Forbid();
                }

                var result = new
                {
                    coupon.Id,
                    coupon.Code,
                    coupon.Title,
                    coupon.Description,
                    coupon.Discount,
                    coupon.IsPercentage,
                    coupon.ExpirationDate,
                    coupon.Active,
                    coupon.Stock,
                    coupon.RestaurantId,
                    Restaurant = coupon.Restaurant != null ? new { coupon.Restaurant.Id, coupon.Restaurant.TradeName } : null,
                    coupon.UserId,
                    User = coupon.User != null ? new { coupon.User.Id, coupon.User.Name, coupon.User.Email } : null
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving coupon {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpPost("create")]
        public async Task<ActionResult<Coupon>> Create([FromBody] Coupon coupon)
        {
            try
            {
                if (coupon == null)
                    return BadRequest(new { message = "Coupon cannot be null" });

                if (string.IsNullOrWhiteSpace(coupon.Code))
                    return BadRequest(new { message = "Code is required" });

                if (coupon.RestaurantId <= 0)
                    return BadRequest(new { message = "RestaurantId must be valid" });

                var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == coupon.RestaurantId);
                if (!restaurantExists)
                    return BadRequest(new { message = $"Restaurant with ID {coupon.RestaurantId} does not exist" });

                // RBAC: verify the restaurant belongs to the authenticated user or is Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin")
                {
                    var restaurant = await _context.Restaurants.FindAsync(coupon.RestaurantId);
                    if (restaurant == null || restaurant.UserId.ToString() != userIdClaim)
                        return Forbid();
                }

                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Coupon created: {coupon.Id}");
                return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating coupon");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Coupon coupon)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });
                if (id != coupon.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var existingCoupon = await _context.Coupons.FindAsync(id);
                if (existingCoupon == null)
                    return NotFound(new { message = $"Coupon with ID {id} not found" });

                // RBAC: verify ownership
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin")
                {
                    if (existingCoupon.RestaurantId.HasValue)
                    {
                        var restaurant = await _context.Restaurants.FindAsync(existingCoupon.RestaurantId.Value);
                        if (restaurant == null || restaurant.UserId.ToString() != userIdClaim)
                            return Forbid();
                    }
                    else if (existingCoupon.UserId.HasValue && existingCoupon.UserId.ToString() != userIdClaim)
                    {
                        return Forbid();
                    }
                }

                existingCoupon.Code = coupon.Code ?? existingCoupon.Code;
                existingCoupon.Title = coupon.Title ?? existingCoupon.Title;
                existingCoupon.Description = coupon.Description ?? existingCoupon.Description;
                existingCoupon.Discount = coupon.Discount > 0 ? coupon.Discount : existingCoupon.Discount;
                existingCoupon.IsPercentage = coupon.IsPercentage;
                existingCoupon.ExpirationDate = coupon.ExpirationDate ?? existingCoupon.ExpirationDate;
                existingCoupon.Active = coupon.Active;
                existingCoupon.Stock = coupon.Stock;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Coupon updated: {id}");
                return Ok(new { message = "Coupon updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating coupon {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var coupon = await _context.Coupons.FindAsync(id);
                if (coupon == null)
                    return NotFound(new { message = $"Coupon with ID {id} not found" });

                // RBAC: verify ownership
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin")
                {
                    if (coupon.RestaurantId.HasValue)
                    {
                        var restaurant = await _context.Restaurants.FindAsync(coupon.RestaurantId.Value);
                        if (restaurant == null || restaurant.UserId.ToString() != userIdClaim)
                            return Forbid();
                    }
                    else if (coupon.UserId.HasValue && coupon.UserId.ToString() != userIdClaim)
                    {
                        return Forbid();
                    }
                }

                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Coupon deleted: {id}");
                return Ok(new { message = "Coupon deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting coupon {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
