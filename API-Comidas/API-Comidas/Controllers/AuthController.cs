using API_Comidas.Data;
using API_Comidas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Comidas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        // Pre-computed bcrypt hash for timing-attack mitigation (hash of "dummy-timing-mitigation")
        private static readonly string DummyHash = BCrypt.Net.BCrypt.HashPassword("dummy-timing-mitigation-2024");

        public AuthController(AppDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("auth-strict")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Query only by email, verify hash in memory
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            bool passwordValid = false;
            if (user != null)
            {
                try
                {
                    // BCrypt verification only — no plaintext fallback
                    passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
                }
                catch
                {
                    // Invalid hash format — treat as failed, never fallback to plaintext
                    passwordValid = false;
                }
            }
            else
            {
                // Timing-attack mitigation: run a dummy verify so response time is the same
                // whether the email exists or not (~200ms for both cases)
                BCrypt.Net.BCrypt.Verify(dto.Password, DummyHash);
            }

            if (!passwordValid)
            {
                _logger.LogWarning("Intento de login fallido desde IP {RemoteIp}", HttpContext.Connection.RemoteIpAddress);
                return Unauthorized("Credenciales incorrectas.");
            }

            var jwtIssuer = _configuration["JWT_ISSUER"] ?? "api-comidas";
            var jwtAudience = _configuration["JWT_AUDIENCE"] ?? "app-comidas";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.UTF8.GetBytes(_configuration["JWT_SECRET"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role?.Name ?? "")
                }),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // NEVER return password or full user entity
            return Ok(new
            {
                Message = "Inicio de sesión exitoso",
                Token = tokenString,
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Phone,
                    user.Img,
                    Role = user.Role?.Name
                }
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("auth-strict")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.RoleId != 2 && dto.RoleId != 3)
                return BadRequest("RoleId inválido. Debe ser 2 (Business) o 3 (Customer).");

            // Anti-enumeration: if email exists, return same response shape as success
            // (same HTTP status, same fields — UserId=0 signals "no new account created")
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return Ok(new { Message = "Solicitud procesada", UserId = 0, RoleId = 0 });
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Registro exitoso",
                UserId = user.Id,
                RoleId = user.RoleId
            });
        }

        [HttpPost("register-restaurant")]
        [Authorize]
        [EnableRateLimiting("auth-strict")]
        public async Task<IActionResult> RegisterRestaurant([FromBody] CreateRestaurantDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Use userId from JWT claim, NOT from body (anti-IDOR)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Usuario no autenticado.");

            var userId = int.Parse(userIdClaim);

            // If body UserId differs from claim and user is not Admin, reject
            if (dto.UserId != userId && roleClaim != "Admin")
                return Forbid();

            // Use claim userId for all lookups
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            if (user.RoleId != 2)
                return BadRequest("El usuario debe tener rol de Business (2) para registrar un restaurante.");

            if (await _context.Restaurants.AnyAsync(r => r.UserId == userId))
                return BadRequest("Este usuario ya tiene un restaurante registrado.");

            var restaurant = new Restaurant
            {
                TradeName = dto.TradeName,
                CategoryId = dto.CategoryId,
                UserId = userId,
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

            return Ok(new
            {
                Message = "Restaurante registrado exitosamente",
                RestaurantId = restaurant.Id,
                UserId = userId
            });
        }
    }
}
