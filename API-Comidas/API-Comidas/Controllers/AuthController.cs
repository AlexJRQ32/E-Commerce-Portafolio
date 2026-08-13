using API_Comidas.Data;
using API_Comidas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Comidas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
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

            return Ok(new
            {
                Message = "Inicio de sesión exitoso",
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
                Img = string.Empty
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
