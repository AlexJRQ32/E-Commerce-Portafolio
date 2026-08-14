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
    public class DishesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DishesController> _logger;

        public DishesController(AppDbContext context, ILogger<DishesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // === RESTful aliases (new) ===

        [HttpGet("")]
        public Task<ActionResult> ListDishes() => List();

        [HttpGet("{id}")]
        public Task<ActionResult> GetDish(int id) => GetById(id);

        [Authorize(Roles = "Business,Admin")]
        [HttpPost("")]
        public Task<ActionResult<Dish>> CreateDish([FromBody] CreateDishDto dto) => Create(dto);

        [Authorize(Roles = "Business,Admin")]
        [HttpPut("{id}")]
        public Task<IActionResult> UpdateDish(int id, [FromBody] CreateDishDto dto) => Update(id, dto);

        [Authorize(Roles = "Business,Admin")]
        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteDish(int id) => Delete(id);

        // === Original routes (kept for backward compatibility) ===

        [HttpGet("list")]
        public async Task<ActionResult> List()
        {
            try
            {
                var dishes = await _context.Dishes
                    .Include(d => d.Restaurant)
                    .Select(d => new
                    {
                        d.Id,
                        d.Name,
                        d.Price,
                        d.Description,
                        d.Img,
                        d.RestaurantId,
                        Restaurant = d.Restaurant != null ? new { d.Restaurant.Id, d.Restaurant.TradeName } : null
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {dishes.Count} dishes");
                return Ok(dishes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dishes");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var dish = await _context.Dishes
                    .Include(d => d.Restaurant)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (dish == null)
                    return NotFound(new { message = $"Dish with ID {id} not found" });

                var result = new
                {
                    dish.Id,
                    dish.Name,
                    dish.Price,
                    dish.Description,
                    dish.Img,
                    dish.RestaurantId,
                    Restaurant = dish.Restaurant != null ? new { dish.Restaurant.Id, dish.Restaurant.TradeName } : null
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving dish {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpPost("create")]
        public async Task<ActionResult<Dish>> Create([FromBody] CreateDishDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (dto == null)
                    return BadRequest(new { message = "Dish cannot be null" });

                var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == dto.RestaurantId);
                if (!restaurantExists)
                    return BadRequest(new { message = $"Restaurant with ID {dto.RestaurantId} does not exist" });

                // RBAC: verify the dish's restaurant belongs to the authenticated user or is Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin")
                {
                    var restaurant = await _context.Restaurants.FindAsync(dto.RestaurantId);
                    if (restaurant == null || restaurant.UserId.ToString() != userIdClaim)
                        return Forbid();
                }

                var dish = new Dish
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    Img = dto.Img,
                    RestaurantId = dto.RestaurantId
                };

                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Dish created: {dish.Id}");
                return CreatedAtAction(nameof(GetById), new { id = dish.Id }, dish);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dish");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Business,Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateDishDto dto)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });
                if (dto == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null)
                    return NotFound(new { message = $"Dish with ID {id} not found" });

                // RBAC: verify ownership of CURRENT restaurant
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin")
                {
                    var restaurant = await _context.Restaurants.FindAsync(dish.RestaurantId);
                    if (restaurant == null || restaurant.UserId.ToString() != userIdClaim)
                        return Forbid();

                    // If RestaurantId is changing, verify destination restaurant also belongs to user
                    if (dto.RestaurantId != dish.RestaurantId)
                    {
                        var destRestaurant = await _context.Restaurants.FindAsync(dto.RestaurantId);
                        if (destRestaurant == null || destRestaurant.UserId.ToString() != userIdClaim)
                            return Forbid("No tienes permiso para mover platos a ese restaurante.");
                    }
                }

                dish.Name = dto.Name;
                dish.Price = dto.Price;
                dish.Description = dto.Description;
                dish.Img = dto.Img;
                dish.RestaurantId = dto.RestaurantId;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Dish updated: {id}");
                return Ok(new { message = "Dish updated successfully" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error updating dish {id}");
                return StatusCode(500, new { message = "Concurrency error" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating dish {id}");
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

                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null)
                    return NotFound(new { message = $"Dish with ID {id} not found" });

                // RBAC: verify ownership
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin")
                {
                    var restaurant = await _context.Restaurants.FindAsync(dish.RestaurantId);
                    if (restaurant == null || restaurant.UserId.ToString() != userIdClaim)
                        return Forbid();
                }

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Dish deleted: {id}");
                return Ok(new { message = "Dish deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting dish {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
