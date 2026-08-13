using API_Comidas.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_Comidas.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Comidas.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GeneralDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GeneralDataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Categories")]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("PaymentMethods")]
        public async Task<ActionResult<List<PaymentMethod>>> GetPaymentMethods()
        {
            return await _context.PaymentMethods.ToListAsync();
        }

        [HttpGet("Roles")]
        public async Task<ActionResult<List<Role>>> GetRoles()
        {
            var roles = await _context.Roles.Where(r => r.Id == 2 || r.Id == 3).ToListAsync();
            return Ok(roles);
        }
    }
}
