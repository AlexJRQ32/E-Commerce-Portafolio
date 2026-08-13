using API_Comidas.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_Comidas.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Comidas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GeneralDataController(AppDbContext context)
        {
            _context = context;
        }

        // Routes: categories, payment-methods, roles
        // ASP.NET routing is case-insensitive, so /api/GeneralData/Categories also matches "categories"
        // The frontend uses lowercase kebab-case: /api/generaldata/payment-methods

        [HttpGet("categories")]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("payment-methods")]
        public async Task<ActionResult<List<PaymentMethod>>> GetPaymentMethods()
        {
            return await _context.PaymentMethods.ToListAsync();
        }

        [HttpGet("roles")]
        public async Task<ActionResult<List<Role>>> GetRoles()
        {
            var roles = await _context.Roles.Where(r => r.Id == 2 || r.Id == 3).ToListAsync();
            return Ok(roles);
        }
    }
}
