using API_Comidas.Data;
using API_Comidas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API_Comidas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RestaurantsController> _logger;

        public RestaurantsController(AppDbContext context, ILogger<RestaurantsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> List()
        {
            try
            {
                var restaurants = await _context.Restaurants
                    .Include(r => r.Category)
                    .Include(r => r.User)
                    .Include(r => r.Dishes)
                    .Include(r => r.Coupons)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {restaurants.Count} restaurants");
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving restaurants");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<Restaurant>> GetById(int id)
        {
            try
            {
                var restaurant = await _context.Restaurants
                    .Include(r => r.Category)
                    .Include(r => r.User)
                    .Include(r => r.Dishes)
                    .Include(r => r.Coupons)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (restaurant == null)
                {
                    return NotFound(new { message = $"Restaurant with ID {id} not found" });
                }

                return Ok(restaurant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving restaurant {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<Restaurant>> Create([FromBody] CreateRestaurantDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { message = "Restaurant data cannot be null" });
                }

                if (string.IsNullOrWhiteSpace(dto.TradeName))
                {
                    return BadRequest(new { message = "TradeName is required" });
                }

                if (dto.CategoryId <= 0)
                {
                    return BadRequest(new { message = "CategoryId must be valid" });
                }

                if (dto.UserId <= 0)
                {
                    return BadRequest(new { message = "UserId must be valid" });
                }

                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
                if (!categoryExists)
                {
                    return BadRequest(new { message = $"Category with ID {dto.CategoryId} does not exist" });
                }

                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
                if (!userExists)
                {
                    return BadRequest(new { message = $"User with ID {dto.UserId} does not exist" });
                }

                var restaurant = new Restaurant
                {
                    TradeName = dto.TradeName,
                    CategoryId = dto.CategoryId,
                    UserId = dto.UserId,
                    Address = dto.Address ?? string.Empty,
                    OpeningTime = dto.OpeningTime ?? "08:00",
                    ClosingTime = dto.ClosingTime ?? "22:00",
                    Img = string.Empty,
                    Rating = "5.0",
                    IsOpen = true,
                    DeliveryFee = 0m,
                    DeliveryTime = "30-45 min"
                };

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Restaurant created: {restaurant.Id}");
                return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating restaurant");
                return StatusCode(500, new { message = "Database error", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating restaurant");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRestaurantDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { message = "Restaurant data cannot be null" });
                }

                var restaurant = await _context.Restaurants.FindAsync(id);
                if (restaurant == null)
                {
                    return NotFound(new { message = $"Restaurant with ID {id} not found" });
                }

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

                if (!string.IsNullOrWhiteSpace(dto.OpeningTime))
                    restaurant.OpeningTime = dto.OpeningTime;

                if (!string.IsNullOrWhiteSpace(dto.ClosingTime))
                    restaurant.ClosingTime = dto.ClosingTime;

                if (!string.IsNullOrWhiteSpace(dto.Img))
                    restaurant.Img = dto.Img;

                if (dto.IsOpen.HasValue)
                    restaurant.IsOpen = dto.IsOpen.Value;

                _context.Entry(restaurant).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Restaurant updated: {id}");
                return Ok(new { message = "Restaurant updated successfully", restaurant });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error updating restaurant {id}");
                return StatusCode(500, new { message = "Concurrency error", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating restaurant {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var restaurant = await _context.Restaurants.FindAsync(id);
                if (restaurant == null)
                {
                    return NotFound(new { message = $"Restaurant with ID {id} not found" });
                }

                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Restaurant deleted: {id}");
                return Ok(new { message = "Restaurant deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting restaurant {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
