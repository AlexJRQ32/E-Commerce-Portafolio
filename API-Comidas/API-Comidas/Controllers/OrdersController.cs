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

                // NEW-4 (FIX): IDOR — allow customer, Admin, OR business owner of the restaurant
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                bool isCustomer = userIdClaim == order.CustomerId.ToString();
                bool isAdmin = roleClaim == "Admin";
                bool isBusinessOwner = order.RestaurantRef?.UserId.ToString() == userIdClaim;

                if (!isCustomer && !isAdmin && !isBusinessOwner)
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

                // Validate FK string references — both are REQUIRED
                if (string.IsNullOrWhiteSpace(order.AddressId))
                    return BadRequest(new { message = "AddressId is required" });

                if (string.IsNullOrWhiteSpace(order.PaymentMethodId))
                    return BadRequest(new { message = "PaymentMethodId is required" });

                // N2 (FIX): Validate address ownership — address must belong to the authenticated user
                var addressExists = await _context.Addresses.AnyAsync(a => a.Id == order.AddressId && a.UserId == claimUserId);
                if (!addressExists)
                    return BadRequest(new { message = "Address does not belong to the authenticated user" });

                var paymentMethodExists = await _context.PaymentMethods.AnyAsync(p => p.Id == order.PaymentMethodId);
                if (!paymentMethodExists)
                    return BadRequest(new { message = $"PaymentMethod with ID '{order.PaymentMethodId}' does not exist" });

                var customerExists = await _context.Users.AnyAsync(u => u.Id == order.CustomerId);
                if (!customerExists)
                    return BadRequest(new { message = $"Customer with ID {order.CustomerId} does not exist" });

                var restaurantExists = await _context.Restaurants.AnyAsync(r => r.Id == order.RestaurantId);
                if (!restaurantExists)
                    return BadRequest(new { message = $"Restaurant with ID {order.RestaurantId} does not exist" });

                // Order must have at least one item
                if (order.Items == null || order.Items.Count == 0)
                    return BadRequest(new { message = "Order must have at least one item" });

                // Recalculate total server-side from REAL dish prices (anti-tampering)
                foreach (var item in order.Items)
                {
                    var dish = await _context.Dishes.FindAsync(item.DishId);
                    if (dish == null)
                        return BadRequest(new { message = $"Dish with ID {item.DishId} does not exist" });
                    item.Price = dish.Price; // Use real price from DB, ignore client price
                }

                order.Total = order.Items.Sum(i => i.Price * i.Quantity);

                // N1 (FIX): Full coupon validation — existence, active, stock, expiration, restaurant ownership, and stock decrement
                if (!string.IsNullOrEmpty(order.CouponCodeApplied))
                {
                    var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == order.CouponCodeApplied);
                    if (coupon == null)
                        return BadRequest(new { message = $"Coupon '{order.CouponCodeApplied}' not found" });
                    if (!coupon.Active)
                        return BadRequest(new { message = "Coupon is not active" });
                    if (!coupon.Stock.HasValue || coupon.Stock <= 0)
                        return BadRequest(new { message = "Coupon has no stock" });
                    // ExpirationDate is string in ISO format "yyyy-MM-dd"
                    if (coupon.ExpirationDate.CompareTo(DateTime.Today.ToString("yyyy-MM-dd")) < 0)
                        return BadRequest(new { message = "Coupon expired" });
                    // Validate restaurant ownership: if coupon has a RestaurantId, it must match the order's restaurant
                    if (coupon.RestaurantId.HasValue && coupon.RestaurantId != order.RestaurantId)
                        return BadRequest(new { message = "Coupon does not belong to this restaurant" });

                    // Apply discount
                    var discount = coupon.IsPercentage
                        ? order.Total * (coupon.Discount / 100m)
                        : coupon.Discount;
                    order.Total = Math.Max(0, order.Total - discount);

                    // Decrement coupon stock (same SaveChangesAsync as the order)
                    coupon.Stock = coupon.Stock - 1;
                    _context.Coupons.Update(coupon);
                }

                if (order.Total <= 0)
                    return BadRequest(new { message = "Total must be greater than 0" });

                // N6 (FIX): Force Status, Date, Time — prevent mass assignment from client body
                order.Status = "Pendiente";
                order.Date = DateTime.Now.ToString("yyyy-MM-dd");
                order.Time = DateTime.Now.ToString("HH:mm");

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

                var existingOrder = await _context.Orders
                    .Include(o => o.RestaurantRef)
                    .FirstOrDefaultAsync(o => o.Id == id);
                if (existingOrder == null)
                    return NotFound(new { message = $"Order with ID {id} not found" });

                if (order == null)
                    return BadRequest(new { message = "Order data cannot be null" });

                // Ownership — customer, Admin, OR restaurant owner (business)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Usuario no autenticado.");

                var claimUserId = int.Parse(userIdClaim);
                bool isCustomer = claimUserId == existingOrder.CustomerId;
                bool isAdmin = roleClaim == "Admin";
                bool isBusinessOwner = existingOrder.RestaurantRef?.UserId == claimUserId;

                if (!isCustomer && !isAdmin && !isBusinessOwner)
                    return Forbid();

                // Only Status is editable via update. Total is immutable after creation.
                if (string.IsNullOrEmpty(order.Status))
                    return NoContent(); // Nothing to update

                // Validate status values
                var validStatuses = new[] { "Pendiente", "En proceso", "Entregado", "Cancelado" };
                if (!validStatuses.Contains(order.Status))
                    return BadRequest(new { message = $"Invalid status. Valid values: {string.Join(", ", validStatuses)}" });

                // NEW-2 (FIX): State machine — validate status transitions
                string currentStatus = existingOrder.Status;
                string newStatus = order.Status;

                // Define allowed transitions: from → [allowed targets]
                var allowedTransitions = new Dictionary<string, HashSet<string>>
                {
                    { "Pendiente", new HashSet<string> { "En proceso", "Cancelado" } },
                    { "En proceso", new HashSet<string> { "Entregado", "Cancelado" } },
                    { "Entregado", new HashSet<string> { "Cancelado" } }, // Only Admin can cancel delivered
                    { "Cancelado", new HashSet<string>() } // Terminal state
                };

                if (!allowedTransitions.TryGetValue(currentStatus, out var allowed) || !allowed.Contains(newStatus))
                {
                    return BadRequest(new { message = $"Invalid status transition from {currentStatus} to {newStatus}" });
                }

                // Role-specific restrictions on top of the state machine:
                // Customer: can only cancel (to "Cancelado") from Pendiente or En proceso
                if (isCustomer && !isAdmin)
                {
                    if (newStatus != "Cancelado")
                        return BadRequest(new { message = "Customers can only cancel orders" });
                }

                // Business owner: cannot cancel from "Entregado" (only Admin can rectify)
                if (isBusinessOwner && !isAdmin)
                {
                    if (currentStatus == "Entregado" && newStatus == "Cancelado")
                        return BadRequest(new { message = "Only administrators can cancel a delivered order" });
                    // Business cannot cancel from Pendiente — they can only process
                    if (currentStatus == "Pendiente" && newStatus == "Cancelado")
                        return BadRequest(new { message = "Business owners cannot cancel pending orders" });
                }

                existingOrder.Status = newStatus;

                // Total, Date, Time, CouponCodeApplied are NOT editable after creation
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Order updated: {id}, status: {newStatus}");
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
