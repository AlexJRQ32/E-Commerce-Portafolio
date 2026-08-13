using API_Comidas.Data;
using API_Comidas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace API_Comidas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

            if (user == null)
                return Unauthorized("Credenciales incorrectas.");

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
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Message = "Inicio de sesión exitoso",
                Token = tokenString,
                User = user
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El correo electrónico ya está registrado.");

            if (dto.RoleId != 2 && dto.RoleId != 3)
                return BadRequest("RoleId inválido. Debe ser 2 (Business) o 3 (Customer).");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
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
        public async Task<IActionResult> RegisterRestaurant([FromBody] CreateRestaurantDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            if (user.RoleId != 2)
                return BadRequest("El usuario debe tener rol de Business (2) para registrar un restaurante.");

            if (await _context.Restaurants.AnyAsync(r => r.UserId == dto.UserId))
                return BadRequest("Este usuario ya tiene un restaurante registrado.");

            var restaurant = new Restaurant
            {
                TradeName = dto.TradeName,
                CategoryId = dto.CategoryId,
                UserId = user.Id,
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
                UserId = user.Id
            });
        }
    }
}
