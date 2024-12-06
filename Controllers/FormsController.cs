using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PfeProject.Data;
using PfeProject.Models;

namespace PfeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FormsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm([FromBody] FormConfiguration formConfig)
        {
            if (formConfig == null || formConfig.Fields == null || !formConfig.Fields.Any())
                return BadRequest("Invalid form configuration.");

            _context.FormConfigurations.Add(formConfig);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetForm), new { id = formConfig.Id }, formConfig);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetForm(string id)
        {
            var formConfig = await _context.FormConfigurations
                .Include(f => f.Fields)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (formConfig == null)
                return NotFound();

            return Ok(formConfig);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllForms()
        {
            var forms = await _context.FormConfigurations.Include(f => f.Fields).ToListAsync();
            return Ok(forms);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(string id, [FromBody] FormConfiguration formConfig)
        {
            if (id != formConfig.Id)
                return BadRequest("Form ID mismatch.");

            var existingForm = await _context.FormConfigurations.Include(f => f.Fields)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (existingForm == null)
                return NotFound();

            existingForm.Name = formConfig.Name;
            existingForm.Fields = formConfig.Fields;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var formConfig = await _context.FormConfigurations.FindAsync(id);
            if (formConfig == null)
                return NotFound();

            _context.FormConfigurations.Remove(formConfig);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFormsByUser(string userId)
        {
            var forms = await _context.FormConfigurations
                .Where(f => f.CreatedByUserId == userId)
                .Include(f => f.Fields)
                .ToListAsync();

            if (forms == null || !forms.Any())
                return NotFound("No forms found for this user.");

            return Ok(forms);
        }
    }

}
