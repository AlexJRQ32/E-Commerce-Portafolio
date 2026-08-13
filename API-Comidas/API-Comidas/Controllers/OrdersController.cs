using API_Comidas.Data;
using API_Comidas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API_Comidas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(AppDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Order>>> List()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.Address)
                    .Include(o => o.RestaurantRef)
                    .Include(o => o.Items)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {orders.Count} orders");
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("GetById/{userId}")]
        public async Task<ActionResult<List<Order>>> GetById(int Id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.Address)
                    .Include(o => o.RestaurantRef)
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == Id);

                if (order == null)
                {
                    return NotFound(new { message = $"Order with ID {Id} not found" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order {Id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest(new { message = "Order cannot be null" });
                }

                if (order.CustomerId <= 0)
                {
                    return BadRequest(new { message = "CustomerId must be valid" });
                }

                if (order.RestaurantId <= 0)
                {
                    return BadRequest(new { message = "RestaurantId must be valid" });
                }

                if (order.Total <= 0)
                {
                    return BadRequest(new { message = "Total must be greater than 0" });
                }

                var customerExists = await _context.Users.AnyAsync(u => u.Id == order.CustomerId);
                if (!customerExists)
                {
                    return BadRequest(new { message = $"Customer with ID {order.CustomerId} does not exist" });
                }

                var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == order.RestaurantId);
                if (!restaurantExists)
                {
                    return BadRequest(new { message = $"Restaurant with ID {order.RestaurantId} does not exist" });
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order created: {order.Id}");
                return CreatedAtAction(nameof(GetById), new { Id = order.Id }, order);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating order");
                return StatusCode(500, new { message = "Database error", error = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Order updated: {id}");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error updating order {id}");
                return StatusCode(500, new { message = "Concurrency error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order deleted: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}