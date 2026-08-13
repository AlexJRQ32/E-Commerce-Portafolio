    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_Comidas.Data;
using Microsoft.EntityFrameworkCore;
using API_Comidas.Models;

namespace API_Comidas.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<User>>> List()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found" });

            return Ok(user);
        }

        [HttpPost("create")]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating user: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] User user)
        {
            try
            {
                if (user == null)
                    return BadRequest("Invalid data.");

                if (id != user.Id)
                    return BadRequest("ID mismatch.");

                User us = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

                if (us == null)
                    return NotFound("User not found.");

                us.Name = user.Name;
                us.Email = user.Email;
                us.Password = user.Password;
                us.Phone = user.Phone;
                us.RoleId = user.RoleId;

                _context.Users.Update(us);
                await _context.SaveChangesAsync();
                return Ok("User updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating user: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                User us = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (us == null)
                    return NotFound("User not found.");

                _context.Users.Remove(us);
                await _context.SaveChangesAsync();
                return Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting user: {ex.Message}");
            }
        }
    }
}