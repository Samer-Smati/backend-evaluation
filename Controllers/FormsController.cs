using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PfeProject.Data;
using PfeProject.Dtos;
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

            // Map to DTO
            var formConfigDto = new FormConfigurationDto
            {
                Id = formConfig.Id,
                Name = formConfig.Name,
                CreatedByUserId = formConfig.CreatedByUserId,
                Fields = formConfig.Fields
                    .OrderBy(f => f.Order)
                    .Select(field => new FormFieldDto
                    {
                        Id = field.Id,
                        FieldType = field.FieldType,
                        Label = field.Label,
                        Value = field.Value,
                        IsRequired = field.IsRequired,
                        Placeholder = field.Placeholder,
                        Order = field.Order
                    })
                    .ToList()
            };

            return Ok(formConfigDto);
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllForms()
        {
            var forms = await _context.FormConfigurations
                .Include(f => f.Fields)
                .ToListAsync();

            // Map to DTO
            var formDtos = forms.Select(form => new FormConfigurationDto
            {
                Id = form.Id,
                Name = form.Name,
                CreatedByUserId = form.CreatedByUserId,
                Fields = form.Fields
                    .OrderBy(field => field.Order)
                    .Select(field => new FormFieldDto
                    {
                        Id = field.Id,
                        FieldType = field.FieldType,
                        Label = field.Label,
                        Value = field.Value,
                        IsRequired = field.IsRequired,
                        Placeholder = field.Placeholder,
                        Order = field.Order
                    })
                    .ToList()
            }).ToList();

            return Ok(formDtos);
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

            var existingFieldIds = existingForm.Fields.Select(f => f.Id).ToList();

            foreach (var field in formConfig.Fields)
            {
                if (existingFieldIds.Contains(field.Id))
                {
                    
                    var existingField = existingForm.Fields.First(f => f.Id == field.Id);
                    existingField.FieldType = field.FieldType;
                    existingField.Label = field.Label;
                    existingField.Value = field.Value;
                    existingField.IsRequired = field.IsRequired;
                    existingField.Placeholder = field.Placeholder;
                    existingField.Order = field.Order;
                }
                else
                {
                    // Add new field
                    existingForm.Fields.Add(new FormField
                    {
                        Id = field.Id, // Make sure Id is set appropriately if required
                        FieldType = field.FieldType,
                        Label = field.Label,
                        Value = field.Value,
                        IsRequired = field.IsRequired,
                        Placeholder = field.Placeholder,
                        Order = field.Order
                    });
                }
            }
            var fieldsToRemove = existingForm.Fields.Where(f => !formConfig.Fields.Select(fc => fc.Id).Contains(f.Id)).ToList();
            foreach (var field in fieldsToRemove)
            {
                _context.FormFields.Remove(field);
            }

            await _context.SaveChangesAsync();
            return Ok();
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

            // Map to DTO
            var formDtos = forms.Select(form => new FormConfigurationDto
            {
                Id = form.Id,
                Name = form.Name,
                CreatedByUserId = form.CreatedByUserId,
                Fields = form.Fields
                    .OrderBy(field => field.Order)
                    .Select(field => new FormFieldDto
                    {
                        Id = field.Id,
                        FieldType = field.FieldType,
                        Label = field.Label,
                        Value = field.Value,
                        IsRequired = field.IsRequired,
                        Placeholder = field.Placeholder,
                        Order = field.Order
                    })
                    .ToList()
            }).ToList();

            return Ok(formDtos);
        }

    }

}
