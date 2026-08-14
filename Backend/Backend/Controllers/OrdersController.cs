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
                        Restaurant = o.RestaurantRef,
                        o.Status,
                        o.CreatedAt,
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
                        Restaurant = o.RestaurantRef,
                        o.Status,
                        o.CreatedAt,
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
                        Restaurant = o.RestaurantRef,
                        o.Status,
                        o.CreatedAt,
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
                    Restaurant = order.RestaurantRef,
                    order.Status,
                    order.CreatedAt,
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

                // Validate FK int references — both are REQUIRED
                if (order.AddressId <= 0)
                    return BadRequest(new { message = "AddressId is required" });

                if (order.PaymentMethodId <= 0)
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

                order.Subtotal = order.Items.Sum(i => i.Price * i.Quantity);
                order.Total = order.Subtotal; // Will add tax/delivery later if needed

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
                    // ExpirationDate is DateOnly
                    if (coupon.ExpirationDate < DateOnly.FromDateTime(DateTime.Today))
                        return BadRequest(new { message = "Coupon expired" });
                    // Validate restaurant ownership: if coupon has a RestaurantId, it must match the order's restaurant
                    if (coupon.RestaurantId.HasValue && coupon.RestaurantId != order.RestaurantId)
                        return BadRequest(new { message = "Coupon does not belong to this restaurant" });

                    // Apply discount
                    var discount = coupon.IsPercentage
                        ? order.Subtotal * (coupon.Discount / 100m)
                        : coupon.Discount;
                    order.Total = Math.Max(0, order.Subtotal - discount);

                    // Decrement coupon stock (same SaveChangesAsync as the order)
                    coupon.Stock = coupon.Stock - 1;
                    _context.Coupons.Update(coupon);
                }

                if (order.Total <= 0)
                    return BadRequest(new { message = "Total must be greater than 0" });

                // N6 (FIX): Force Status, CreatedAt — prevent mass assignment from client body
                order.Status = Order.OrderStatus.Pending;
                order.CreatedAt = DateTime.UtcNow;

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

                if (order == null)
                    return BadRequest(new { message = "Order data cannot be null" });

                // Only Status is editable via update. Total is immutable after creation.
                // order.Status is already an enum (model binder converts string -> enum)
                if (order.Status == default)
                    return NoContent(); // Nothing to update

                var newStatusEnum = order.Status;
                var currentStatus = existingOrder.Status;

                // Define allowed transitions: from → [allowed targets]
                var allowedTransitions = new Dictionary<Order.OrderStatus, HashSet<Order.OrderStatus>>
                {
                    { Order.OrderStatus.Pending, new HashSet<Order.OrderStatus> { Order.OrderStatus.Confirmed, Order.OrderStatus.Cancelled } },
                    { Order.OrderStatus.Confirmed, new HashSet<Order.OrderStatus> { Order.OrderStatus.Preparing, Order.OrderStatus.Cancelled } },
                    { Order.OrderStatus.Preparing, new HashSet<Order.OrderStatus> { Order.OrderStatus.Ready, Order.OrderStatus.Cancelled } },
                    { Order.OrderStatus.Ready, new HashSet<Order.OrderStatus> { Order.OrderStatus.OutForDelivery, Order.OrderStatus.Cancelled } },
                    { Order.OrderStatus.OutForDelivery, new HashSet<Order.OrderStatus> { Order.OrderStatus.Delivered, Order.OrderStatus.Cancelled } },
                    { Order.OrderStatus.Delivered, new HashSet<Order.OrderStatus> { Order.OrderStatus.Cancelled } }, // Only Admin can cancel delivered
                    { Order.OrderStatus.Cancelled, new HashSet<Order.OrderStatus>() } // Terminal state
                };

                if (!allowedTransitions.TryGetValue(currentStatus, out var allowed) || !allowed.Contains(newStatusEnum))
                {
                    return BadRequest(new { message = $"Invalid status transition from {currentStatus} to {newStatusEnum}" });
                }

                // Role-specific restrictions on top of the state machine:
                // Customer: can only cancel (to "Cancelled") from Pending or Confirmed — NOT from Delivered
                if (isCustomer && !isAdmin)
                {
                    if (newStatusEnum != Order.OrderStatus.Cancelled)
                        return BadRequest(new { message = "Customers can only cancel their orders" });
                    if (currentStatus == Order.OrderStatus.Delivered)
                        return BadRequest(new { message = "Only administrators can cancel a delivered order" });
                    if (currentStatus == Order.OrderStatus.Cancelled)
                        return BadRequest(new { message = "Order is already cancelled" });
                }

                // Business owner: cannot cancel from "Delivered" (only Admin can rectify)
                if (isBusinessOwner && !isAdmin)
                {
                    if (currentStatus == Order.OrderStatus.Delivered && newStatusEnum == Order.OrderStatus.Cancelled)
                        return BadRequest(new { message = "Only administrators can cancel a delivered order" });
                    // Business cannot cancel from Pending/Confirmed — they can only process
                    if ((currentStatus == Order.OrderStatus.Pending || currentStatus == Order.OrderStatus.Confirmed) && newStatusEnum == Order.OrderStatus.Cancelled)
                        return BadRequest(new { message = "Business owners cannot cancel pending/confirmed orders" });
                }

                existingOrder.Status = newStatusEnum;
                existingOrder.UpdatedAt = DateTime.UtcNow;

                // Total, CreatedAt, CouponCodeApplied are NOT editable after creation
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Order updated: {id}, status: {newStatusEnum}");
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