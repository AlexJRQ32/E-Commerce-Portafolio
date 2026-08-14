using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("general")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // === RESTful aliases (new) ===

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public Task<ActionResult> ListUsers() => List();

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isOwner = userIdClaim == id.ToString();
            bool isAdmin = roleClaim == "Admin";

            // Unauthenticated or not owner/admin → public profile only
            if (!User.Identity?.IsAuthenticated == true || (!isOwner && !isAdmin))
            {
                return await GetPublicProfile(id);
            }

            // Owner or Admin → full profile (without password)
            return await GetFullProfile(id);
        }

        private async Task<ActionResult> GetPublicProfile(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Invalid ID" });

            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Img
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found" });

            return Ok(user);
        }

        private async Task<ActionResult> GetFullProfile(int id)
        {
            if (id <= 0) return BadRequest(new { message = "Invalid ID" });

            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.Img,
                    u.RoleId
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found" });

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("")]
        public Task<ActionResult> CreateUser([FromBody] User user) => Create(user);

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] User user)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isAdmin = roleClaim == "Admin";
            bool isOwner = userIdClaim == id.ToString();

            if (!isAdmin && !isOwner)
            {
                if (User.Identity?.IsAuthenticated == true)
                    return Forbid();
                return Unauthorized("Authentication required");
            }

            return await Update(id, user, isAdmin, isOwner);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteUser(int id) => Delete(id);

        [HttpGet("{userId}/addresses")]
        public async Task<ActionResult> GetUserAddresses(int userId)
        {
            // Allow the user themselves or Admin
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (roleClaim != "Admin" && userIdClaim != userId.ToString())
            {
                if (User.Identity?.IsAuthenticated == true)
                    return Forbid();
                return Unauthorized("Authentication required");
            }

            try
            {
                var addresses = await _context.Addresses
                    .Where(a => a.UserId == userId)
                    .Select(a => new
                    {
                        a.Id,
                        a.Name,
                        a.UserId
                    })
                    .ToListAsync();

                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // NEW-5 (FIX): POST endpoint to create addresses
        [HttpPost("{userId}/addresses")]
        [Authorize]
        public async Task<ActionResult> CreateUserAddress(int userId, [FromBody] CreateAddressDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only the user themselves or Admin can create an address for that user
            if (roleClaim != "Admin" && userIdClaim != userId.ToString())
                return Forbid();

            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Address name is required" });

            // Sanitize Name: letters, numbers, spaces, accents, n-tilde, hyphens, periods, commas
            if (!Regex.IsMatch(dto.Name, @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-\.\,]+$"))
                return BadRequest(new { message = "Address name can only contain letters, numbers, spaces, accents, hyphens, periods, and commas" });

            var address = new Address
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = dto.Name,
                UserId = userId
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserAddresses), new { userId }, new
            {
                address.Id,
                address.Name,
                address.UserId
            });
        }

        // === Original routes (kept for backward compatibility) ===

        [Authorize(Roles = "Admin")]
        [HttpGet("list")]
        public async Task<ActionResult> List()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.Img,
                    u.RoleId
                })
                .ToListAsync();
            return Ok(users);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isOwner = userIdClaim == id.ToString();
            bool isAdmin = roleClaim == "Admin";

            if (User.Identity?.IsAuthenticated == true && (isOwner || isAdmin))
            {
                return await GetFullProfile(id);
            }

            return await GetPublicProfile(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] User user)
        {
            try
            {
                if (user == null)
                    return BadRequest(new { message = "Invalid data." });

                // Sanitize Name: only letters, spaces, accents, n-tilde, hyphens, periods
                if (!Regex.IsMatch(user.Name, @"^[a-zA-Z\u00C0-\u017F\s\-\.]+$"))
                    return BadRequest(new { message = "Name can only contain letters, spaces, accents, n-tilde, hyphens, and periods" });

                // Hash password before saving
                if (!string.IsNullOrEmpty(user.Password))
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                else
                    return BadRequest(new { message = "Password is required" });

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Phone,
                    user.Img,
                    user.RoleId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public Task<ActionResult> UpdateAdmin(int id, [FromBody] User user) => Update(id, user, isAdmin: true, isOwner: true);

        private async Task<ActionResult> Update(int id, [FromBody] User user, bool isAdmin, bool isOwner)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });
                if (user == null)
                    return BadRequest("Invalid data.");

                if (id != user.Id)
                    return BadRequest("ID mismatch.");

                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    return NotFound("User not found.");

                // Sanitize Name
                if (!string.IsNullOrEmpty(user.Name) && !Regex.IsMatch(user.Name, @"^[a-zA-Z\u00C0-\u017F\s\-\.]+$"))
                    return BadRequest(new { message = "Name can only contain letters, spaces, accents, n-tilde, hyphens, and periods" });

                existingUser.Name = user.Name ?? existingUser.Name;

                // Non-owner (admin editing another user): can change everything
                // Owner (self-edit): can change Name, Phone, Img, Password — NOT Email or RoleId
                // Admin self-edit: same restrictions as owner for RoleId
                if (isAdmin && !isOwner)
                {
                    // Admin editing another user: full control
                    existingUser.Email = user.Email ?? existingUser.Email;
                    existingUser.Phone = user.Phone ?? existingUser.Phone;
                    existingUser.RoleId = user.RoleId > 0 ? user.RoleId : existingUser.RoleId;
                }
                else
                {
                    // Self-edit (owner) or non-admin: protect Email and RoleId
                    existingUser.Phone = user.Phone ?? existingUser.Phone;
                    // RoleId is NEVER modifiable by non-admin or self-edit
                    // Email is identifier — only Admin editing another user can change it
                }

                // Only update password if a new one is provided
                if (!string.IsNullOrEmpty(user.Password))
                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                await _context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid ID" });

                var us = await _context.Users.FindAsync(id);
                if (us == null)
                    return NotFound("User not found.");

                _context.Users.Remove(us);
                await _context.SaveChangesAsync();
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
