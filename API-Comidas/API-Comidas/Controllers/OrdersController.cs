using API_Comidas.Data;
using API_Comidas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API_Comidas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(AppDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // === RESTful aliases (new) ===

        [HttpGet("")]
        public Task<ActionResult> ListOrders() => List();

        [HttpGet("{id}")]
        public Task<ActionResult<Order>> GetOrder(int id) => GetById(id);

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetOrdersByUser(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim != "Admin" && userIdClaim != userId.ToString())
                return Forbid();

            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.Address)
                    .Include(o => o.RestaurantRef)
                    .Include(o => o.Items)
                    .Where(o => o.CustomerId == userId)
                    .Select(o => new
                    {
                        o.Id,
                        o.Restaurant,
                        o.Status,
                        o.Date,
                        o.Time,
                        o.CouponCodeApplied,
                        o.CustomerId,
                        Customer = o.Customer != null ? new { o.Customer.Id, o.Customer.Name, o.Customer.Email } : null,
                        o.PaymentMethodId,
                        PaymentMethod = o.PaymentMethod != null ? new { o.PaymentMethod.Id, o.PaymentMethod.Name } : null,
                        o.AddressId,
                        Address = o.Address != null ? new { o.Address.Id, o.Address.Name } : null,
                        o.Total,
                        o.RestaurantId,
                        RestaurantRef = o.RestaurantRef != null ? new { o.RestaurantRef.Id, o.RestaurantRef.TradeName } : null,
                        Items = o.Items.Select(i => new { i.Id, i.DishId, i.Quantity, i.Price }).ToList()
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult> GetOrdersByRestaurant(int restaurantId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            // Verify ownership of the restaurant or Admin
            if (roleClaim != "Admin")
            {
                var restaurant = await _context.Restaurants.FindAsync(restaurantId);
                if (restaurant == null || restaurant.UserId.ToString() != userIdClaim)
                    return Forbid();
            }

            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.Address)
                    .Include(o => o.RestaurantRef)
                    .Include(o => o.Items)
                    .Where(o => o.RestaurantId == restaurantId)
                    .Select(o => new
                    {
                        o.Id,
                        o.Restaurant,
                        o.Status,
                        o.Date,
                        o.Time,
                        o.CouponCodeApplied,
                        o.CustomerId,
                        Customer = o.Customer != null ? new { o.Customer.Id, o.Customer.Name, o.Customer.Email } : null,
                        o.PaymentMethodId,
                        PaymentMethod = o.PaymentMethod != null ? new { o.PaymentMethod.Id, o.PaymentMethod.Name } : null,
                        o.AddressId,
                        Address = o.Address != null ? new { o.Address.Id, o.Address.Name } : null,
                        o.Total,
                        o.RestaurantId,
                        RestaurantRef = o.RestaurantRef != null ? new { o.RestaurantRef.Id, o.RestaurantRef.TradeName } : null,
                        Items = o.Items.Select(i => new { i.Id, i.DishId, i.Quantity, i.Price }).ToList()
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for restaurant {RestaurantId}", restaurantId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // PUT /{id} and DELETE /{id} already exist as RESTful routes below

        // === Original routes (kept for backward compatibility) ===

        [HttpGet("list")]
        public async Task<ActionResult> List()
        {
            try
            {
                // IDOR: filter by authenticated user unless Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                IQueryable<Order> query = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.Address)
                    .Include(o => o.RestaurantRef)
                    .Include(o => o.Items);

                // Non-Admin users only see their own orders
                if (roleClaim != "Admin" && int.TryParse(userIdClaim, out int userId))
                {
                    query = query.Where(o => o.CustomerId == userId);
                }

                var orders = await query
                    .Select(o => new
                    {
                        o.Id,
                        o.Restaurant,
                        o.Status,
                        o.Date,
                        o.Time,
                        o.CouponCodeApplied,
                        o.CustomerId,
                        Customer = o.Customer != null ? new { o.Customer.Id, o.Customer.Name, o.Customer.Email } : null,
                        o.PaymentMethodId,
                        PaymentMethod = o.PaymentMethod != null ? new { o.PaymentMethod.Id, o.PaymentMethod.Name } : null,
                        o.AddressId,
                        Address = o.Address != null ? new { o.Address.Id, o.Address.Name } : null,
                        o.Total,
                        o.RestaurantId,
                        RestaurantRef = o.RestaurantRef != null ? new { o.RestaurantRef.Id, o.RestaurantRef.TradeName } : null,
                        Items = o.Items.Select(i => new { i.Id, i.DishId, i.Quantity, i.Price }).ToList()
                    })
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {orders.Count} orders for user {userIdClaim} (role: {roleClaim})");
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.Address)
                    .Include(o => o.RestaurantRef)
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                    return NotFound(new { message = $"Order with ID {id} not found" });

                // IDOR: verify the authenticated user owns this order or is Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin" && userIdClaim != order.CustomerId.ToString())
                    return Forbid();

                var result = new
                {
                    order.Id,
                    order.Restaurant,
                    order.Status,
                    order.Date,
                    order.Time,
                    order.CouponCodeApplied,
                    order.CustomerId,
                    Customer = order.Customer != null ? new { order.Customer.Id, order.Customer.Name, order.Customer.Email } : null,
                    order.PaymentMethodId,
                    PaymentMethod = order.PaymentMethod != null ? new { order.PaymentMethod.Id, order.PaymentMethod.Name } : null,
                    order.AddressId,
                    Address = order.Address != null ? new { order.Address.Id, order.Address.Name } : null,
                    order.Total,
                    order.RestaurantId,
                    RestaurantRef = order.RestaurantRef != null ? new { order.RestaurantRef.Id, order.RestaurantRef.TradeName } : null,
                    Items = order.Items.Select(i => new { i.Id, i.DishId, i.Quantity, i.Price }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            try
            {
                if (order == null)
                    return BadRequest(new { message = "Order cannot be null" });

                // Force CustomerId from JWT claim (anti-IDOR), unless Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Usuario no autenticado.");

                var claimUserId = int.Parse(userIdClaim);
                if (roleClaim != "Admin")
                {
                    order.CustomerId = claimUserId;
                }
                else if (order.CustomerId <= 0)
                {
                    // Admin must specify a valid CustomerId
                    return BadRequest(new { message = "CustomerId must be valid" });
                }

                if (order.RestaurantId <= 0)
                    return BadRequest(new { message = "RestaurantId must be valid" });

                // Validate FK string references before insert
                if (!string.IsNullOrEmpty(order.AddressId))
                {
                    var addressExists = await _context.Addresses.AnyAsync(a => a.Id == order.AddressId);
                    if (!addressExists)
                        return BadRequest(new { message = $"Address with ID '{order.AddressId}' does not exist" });
                }

                if (!string.IsNullOrEmpty(order.PaymentMethodId))
                {
                    var paymentMethodExists = await _context.PaymentMethods.AnyAsync(p => p.Id.ToString() == order.PaymentMethodId);
                    if (!paymentMethodExists)
                        return BadRequest(new { message = $"PaymentMethod with ID '{order.PaymentMethodId}' does not exist" });
                }

                var customerExists = await _context.Users.AnyAsync(u => u.Id == order.CustomerId);
                if (!customerExists)
                    return BadRequest(new { message = $"Customer with ID {order.CustomerId} does not exist" });

                var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == order.RestaurantId);
                if (!restaurantExists)
                    return BadRequest(new { message = $"Restaurant with ID {order.RestaurantId} does not exist" });

                // Recalculate total server-side from REAL dish prices (anti-tampering)
                if (order.Items != null && order.Items.Count > 0)
                {
                    foreach (var item in order.Items)
                    {
                        var dish = await _context.Dishes.FindAsync(item.DishId);
                        if (dish == null)
                            return BadRequest(new { message = $"Dish with ID {item.DishId} does not exist" });
                        item.Price = dish.Price; // Use real price from DB, ignore client price
                    }

                    order.Total = order.Items.Sum(i => i.Price * i.Quantity);

                    // Optional: apply coupon discount if CouponCodeApplied is set
                    if (!string.IsNullOrEmpty(order.CouponCodeApplied))
                    {
                        var coupon = await _context.Coupons
                            .FirstOrDefaultAsync(c => c.Code == order.CouponCodeApplied && c.Active && c.Stock > 0);
                        if (coupon != null)
                        {
                            var discount = coupon.IsPercentage
                                ? order.Total * (coupon.Discount / 100m)
                                : coupon.Discount;
                            order.Total = Math.Max(0, order.Total - discount);
                        }
                    }
                }

                if (order.Total <= 0)
                    return BadRequest(new { message = "Total must be greater than 0" });

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order created: {order.Id} by user {claimUserId}");
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating order");
                return StatusCode(500, new { message = "Database error" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var existingOrder = await _context.Orders.FindAsync(id);
                if (existingOrder == null)
                    return NotFound(new { message = $"Order with ID {id} not found" });

                if (order == null)
                    return BadRequest(new { message = "Order data cannot be null" });

                if (id != order.Id)
                    return BadRequest(new { message = "ID mismatch" });

                // IDOR: verify ownership or Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin" && userIdClaim != existingOrder.CustomerId.ToString())
                    return Forbid();

                // Only Status is editable via update. Total is immutable after creation.
                // Validate status values
                var validStatuses = new[] { "Pendiente", "En proceso", "Entregado", "Cancelado" };
                if (!string.IsNullOrEmpty(order.Status) && !validStatuses.Contains(order.Status))
                    return BadRequest(new { message = $"Invalid status. Valid values: {string.Join(", ", validStatuses)}" });

                if (!string.IsNullOrEmpty(order.Status))
                    existingOrder.Status = order.Status;

                // Total, Date, Time, CouponCodeApplied are NOT editable after creation
                // (Total is recalculated server-side on creation; Date/Time are set at checkout)

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Order updated: {id}");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Error updating order {id}");
                return StatusCode(500, new { message = "Concurrency error" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                    return NotFound(new { message = $"Order with ID {id} not found" });

                // IDOR: verify ownership or Admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != "Admin" && userIdClaim != order.CustomerId.ToString())
                    return Forbid();

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order deleted: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
