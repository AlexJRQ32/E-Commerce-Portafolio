using API_Comidas.Data;
using API_Comidas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API_Comidas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CouponsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CouponsController> _logger;

        public CouponsController(AppDbContext context, ILogger<CouponsController> _logger)
        {
            _context = context;
            this._logger = _logger;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Coupon>>> List()
        {
            try
            {
                var coupons = await _context.Coupons
                    .Include(c => c.Restaurant)
                    .Include(c => c.User)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {coupons.Count} coupons");
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupons");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<Coupon>> GetById(int id)
        {
            try
            {
                var coupon = await _context.Coupons
                    .Include(c => c.Restaurant)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (coupon == null)
                    return NotFound(new { message = $"Coupon with ID {id} not found" });

                return Ok(coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving coupon {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

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

                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Coupon created: {coupon.Id}");
                return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating coupon");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Coupon coupon)
        {
            try
            {
                if (id != coupon.Id)
                    return BadRequest(new { message = "ID mismatch" });

                _context.Entry(coupon).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Coupon updated: {id}");
                return Ok(new { message = "Coupon updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating coupon {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var coupon = await _context.Coupons.FindAsync(id);
                if (coupon == null)
                    return NotFound(new { message = $"Coupon with ID {id} not found" });

                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Coupon deleted: {id}");
                return Ok(new { message = "Coupon deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting coupon {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
