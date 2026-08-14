using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("general")]
    public class RestaurantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RestaurantsController> _logger;

        public RestaurantsController(AppDbContext context, ILogger<RestaurantsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // === RESTful aliases (new) ===

        [HttpGet("")]
        public Task<ActionResult> ListRestaurants() => List();

        [HttpGet("{id}")]
        public Task<ActionResult> GetRestaurant(int id) => GetById(id);

        [HttpGet("{id}/menu")]
        public async Task<ActionResult> GetRestaurantMenu(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var dishes = await _context.Dishes
                    .Where(d => d.RestaurantId == id)
                    .Select(d => new
                    {
                        d.Id,
                        d.Name,
                        d.Price,
                        d.Description,
                        d.Img,
                        d.RestaurantId
                    })
                    .ToListAsync();

                return Ok(dishes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving menu for restaurant {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetRestaurantsByUser(int userId)
        {
            try
            {
                if (userId <= 0) return BadRequest(new { message = "Invalid userId" });

                var restaurants = await _context.Restaurants
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Category)
                    .Select(r => new
                    {
                        r.Id,
                        r.TradeName,
                        r.Address,
                        r.CategoryId,
                        Category = r.Category != null ? new { r.Category.Id, r.Category.Name } : null,
                        r.UserId,
                        r.OpeningTime,
                        r.ClosingTime,
                        r.Img,
                        r.Rating,
                        r.IsOpen,
                        r.DeliveryFee,
                        r.DeliveryTime,
                        r.Latitude,
                        r.Longitude,
                        r.MinOrderAmount
                    })
                    .ToListAsync();

                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving restaurants for user {userId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpPost("")]
        public Task<ActionResult<Restaurant>> CreateRestaurant([FromBody] CreateRestaurantDto dto) => Create(dto);

        [Authorize(Roles = "Business,Admin")]
        [HttpPut("{id}")]
        public Task<IActionResult> UpdateRestaurant(int id, [FromBody] UpdateRestaurantDto dto) => Update(id, dto);

        [Authorize(Roles = "Business,Admin")]
        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteRestaurant(int id) => Delete(id);

        // === Original routes (kept for backward compatibility) ===

        [HttpGet("list")]
        public async Task<ActionResult> List()
        {
            try
            {
                var restaurants = await _context.Restaurants
                    .Include(r => r.Category)
                    .Include(r => r.Dishes)
                    .Include(r => r.Coupons)
                    .Select(r => new
                    {
                        r.Id,
                        r.TradeName,
                        r.Address,
                        r.CategoryId,
                        Category = r.Category != null ? new { r.Category.Id, r.Category.Name } : null,
                        r.UserId,
                        r.OpeningTime,
                        r.ClosingTime,
                        r.Img,
                        r.Rating,
                        r.IsOpen,
                        r.DeliveryFee,
                        r.DeliveryTime,
                        r.Latitude,
                        r.Longitude,
                        r.MinOrderAmount,
                        Dishes = r.Dishes.Select(d => new { d.Id, d.Name, d.Price, d.Description, d.Img }).ToList(),
                        Coupons = r.Coupons.Select(c => new { c.Id, c.Code, c.Title, c.Discount, c.IsPercentage, c.Active }).ToList(),
                        User = r.User != null ? new { r.User.Id, r.User.Name, r.User.Img } : null
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {restaurants.Count} restaurants");
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving restaurants");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var restaurant = await _context.Restaurants
                    .Include(r => r.Category)
                    .Include(r => r.Dishes)
                    .Include(r => r.Coupons)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (restaurant == null)
                    return NotFound(new { message = $"Restaurant with ID {id} not found" });

                var result = new
                {
                    restaurant.Id,
                    restaurant.TradeName,
                    restaurant.Address,
                    restaurant.CategoryId,
                    Category = restaurant.Category != null ? new { restaurant.Category.Id, restaurant.Category.Name } : null,
                    restaurant.UserId,
                    restaurant.OpeningTime,
                    restaurant.ClosingTime,
                    restaurant.Img,
                    restaurant.Rating,
                    restaurant.IsOpen,
                    restaurant.DeliveryFee,
                    restaurant.DeliveryTime,
                    restaurant.Latitude,
                    restaurant.Longitude,
                    restaurant.MinOrderAmount,
                    Dishes = restaurant.Dishes.Select(d => new { d.Id, d.Name, d.Price, d.Description, d.Img }).ToList(),
                    Coupons = restaurant.Coupons.Select(c => new { c.Id, c.Code, c.Title, c.Discount, c.IsPercentage, c.Active }).ToList(),
                    User = restaurant.User != null ? new { restaurant.User.Id, restaurant.User.Name, restaurant.User.Img } : null
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving restaurant {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpPost("")]
        public async Task<ActionResult<Restaurant>> Create([FromBody] CreateRestaurantDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Restaurant data cannot be null" });

                if (string.IsNullOrWhiteSpace(dto.TradeName))
                    return BadRequest(new { message = "TradeName is required" });

                if (dto.CategoryId <= 0)
                    return BadRequest(new { message = "CategoryId must be valid" });

                if (dto.UserId <= 0)
                    return BadRequest(new { message = "UserId must be valid" });

                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
                if (!categoryExists)
                    return BadRequest(new { message = $"Category with ID {dto.CategoryId} does not exist" });

                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
                if (!userExists)
                    return BadRequest(new { message = $"User with ID {dto.UserId} does not exist" });

                // RBAC: verify the authenticated user owns this restaurant or is Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin" && userIdClaim != dto.UserId.ToString())
                    return Forbid();

                var restaurant = new Restaurant
                {
                    TradeName = dto.TradeName,
                    CategoryId = dto.CategoryId,
                    UserId = dto.UserId,
                    Address = dto.Address ?? string.Empty,
                    OpeningTime = dto.OpeningTime,
                    ClosingTime = dto.ClosingTime,
                    Img = string.Empty,
                    Rating = 5.0m,
                    IsOpen = true,
                    DeliveryFee = dto.DeliveryFee,
                    DeliveryTime = dto.DeliveryTime,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    MinOrderAmount = dto.MinOrderAmount
                };

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Restaurant created: {restaurant.Id}");
                return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating restaurant");
                return StatusCode(500, new { message = "Database error" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating restaurant");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRestaurantDto dto)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });
                if (dto == null)
                    return BadRequest(new { message = "Restaurant data cannot be null" });

                var restaurant = await _context.Restaurants.FindAsync(id);
                if (restaurant == null)
                    return NotFound(new { message = $"Restaurant with ID {id} not found" });

                // RBAC: verify ownership or Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin" && userIdClaim != restaurant.UserId.ToString())
                    return Forbid();

                if (!string.IsNullOrWhiteSpace(dto.TradeName))
                    restaurant.TradeName = dto.TradeName;

                if (!string.IsNullOrWhiteSpace(dto.Address))
                    restaurant.Address = dto.Address;

                if (dto.CategoryId > 0)
                {
                    var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
                    if (!categoryExists)
                        return BadRequest(new { message = $"Category with ID {dto.CategoryId} does not exist" });
                    restaurant.CategoryId = dto.CategoryId;
                }

                if (dto.OpeningTime.HasValue)
                    restaurant.OpeningTime = dto.OpeningTime.Value;

                if (dto.ClosingTime.HasValue)
                    restaurant.ClosingTime = dto.ClosingTime.Value;

                if (!string.IsNullOrWhiteSpace(dto.Img))
                    restaurant.Img = dto.Img;

                if (dto.IsOpen.HasValue)
                    restaurant.IsOpen = dto.IsOpen.Value;

                if (dto.Latitude.HasValue)
                    restaurant.Latitude = dto.Latitude.Value;

                if (dto.Longitude.HasValue)
                    restaurant.Longitude = dto.Longitude.Value;

                if (dto.DeliveryFee.HasValue)
                    restaurant.DeliveryFee = dto.DeliveryFee.Value;

                if (!string.IsNullOrWhiteSpace(dto.DeliveryTime))
                    restaurant.DeliveryTime = dto.DeliveryTime;

                if (dto.MinOrderAmount.HasValue)
                    restaurant.MinOrderAmount = dto.MinOrderAmount.Value;

                if (dto.IsOpen.HasValue)
                    restaurant.IsOpen = dto.IsOpen.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Restaurant updated: {id}");
                return Ok(new { message = "Restaurant updated successfully" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error updating restaurant {id}");
                return StatusCode(500, new { message = "Concurrency error" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating restaurant {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var restaurant = await _context.Restaurants.FindAsync(id);
                if (restaurant == null)
                    return NotFound(new { message = $"Restaurant with ID {id} not found" });

                // RBAC: verify ownership or Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin" && userIdClaim != restaurant.UserId.ToString())
                    return Forbid();

                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Restaurant deleted: {id}");
                return Ok(new { message = "Restaurant deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting restaurant {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}