using API_Comidas.Data;
using API_Comidas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API_Comidas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DishesController> _logger;

        public DishesController(AppDbContext context, ILogger<DishesController> _logger)
        {
            _context = context;
            this._logger = _logger;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Dish>>> List()
        {
            try
            {
                var dishes = await _context.Dishes
                    .Include(d => d.Restaurant)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {dishes.Count} dishes");
                return Ok(dishes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dishes");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<Dish>> GetById(int id)
        {
            try
            {
                var dish = await _context.Dishes
                    .Include(d => d.Restaurant)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (dish == null)
                    return NotFound(new { message = $"Dish with ID {id} not found" });

                return Ok(dish);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving dish {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<Dish>> Create([FromBody] Dish dish)
        {
            try
            {
                if (dish == null)
                    return BadRequest(new { message = "Dish cannot be null" });

                if (string.IsNullOrWhiteSpace(dish.Name))
                    return BadRequest(new { message = "Name is required" });

                if (dish.RestaurantId <= 0)
                    return BadRequest(new { message = "RestaurantId must be valid" });

                var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == dish.RestaurantId);
                if (!restaurantExists)
                    return BadRequest(new { message = $"Restaurant with ID {dish.RestaurantId} does not exist" });

                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Dish created: {dish.Id}");
                return CreatedAtAction(nameof(GetById), new { id = dish.Id }, dish);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dish");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Dish dish)
        {
            try
            {
                if (id != dish.Id)
                    return BadRequest(new { message = "ID mismatch" });

                _context.Entry(dish).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Dish updated: {id}");
                return Ok(new { message = "Dish updated successfully" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error updating dish {id}");
                return StatusCode(500, new { message = "Concurrency error", error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating dish {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null)
                    return NotFound(new { message = $"Dish with ID {id} not found" });

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Dish deleted: {id}");
                return Ok(new { message = "Dish deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting dish {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
