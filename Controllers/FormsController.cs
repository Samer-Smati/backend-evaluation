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

        // Create a new form
        [HttpPost]
        public async Task<IActionResult> CreateForm([FromBody] FormConfigurationDto formConfigDto)
        {
            if (formConfigDto == null || formConfigDto.Fields == null || !formConfigDto.Fields.Any())
                return BadRequest("Invalid form configuration.");

            // Map DTO to Entity
            var formConfig = new FormConfiguration
            {
                Id = Guid.NewGuid().ToString(),
                Name = formConfigDto.Name,
                CreatedByUserId = formConfigDto.CreatedByUserId,
                Fields = formConfigDto.Fields.Select(f => new FormField
                {
                    Id = Guid.NewGuid().ToString(),
                    FieldType = f.FieldType,
                    Label = f.Label,
                    Value = f.Value,
                    IsRequired = f.IsRequired,
                    Placeholder = f.Placeholder,
                    Order = f.Order,
                    Options = f.Options?.Select(o => new FormFieldOption
                    {
                        Label = o.Label,
                        Value = o.Value,
                        Name = o.Name
                    }).ToList()
                }).ToList()
            };

            _context.FormConfigurations.Add(formConfig);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetForm), new { id = formConfig.Id }, formConfigDto);
        }

        // Retrieve a single form by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForm(string id)
        {
            var formConfig = await _context.FormConfigurations
                .Include(f => f.Fields)
                    .ThenInclude(f => f.Options)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (formConfig == null)
                return NotFound();

            // Map to DTO
            var formConfigDto = MapToFormConfigurationDto(formConfig);
            return Ok(formConfigDto);
        }

        // Retrieve all forms
        [HttpGet("all")]
        public async Task<IActionResult> GetAllForms()
        {
            var forms = await _context.FormConfigurations
                .Include(f => f.Fields)
                    .ThenInclude(f => f.Options)
                .ToListAsync();

            var formDtos = forms.Select(MapToFormConfigurationDto).ToList();
            return Ok(formDtos);
        }

        // Update a form by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(string id, [FromBody] FormConfigurationDto formConfigDto)
        {
            if (id != formConfigDto.Id)
                return BadRequest("Form ID mismatch.");

            var existingForm = await _context.FormConfigurations
                .Include(f => f.Fields)
                    .ThenInclude(f => f.Options)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (existingForm == null)
                return NotFound();

            // Update form details
            existingForm.Name = formConfigDto.Name;

            // Update fields
            var existingFieldIds = existingForm.Fields.Select(f => f.Id).ToList();

            foreach (var fieldDto in formConfigDto.Fields)
            {
                if (existingFieldIds.Contains(fieldDto.Id))
                {
                    // Update existing field
                    var existingField = existingForm.Fields.First(f => f.Id == fieldDto.Id);
                    existingField.FieldType = fieldDto.FieldType;
                    existingField.Label = fieldDto.Label;
                    existingField.Value = fieldDto.Value;
                    existingField.IsRequired = fieldDto.IsRequired;
                    existingField.Placeholder = fieldDto.Placeholder;
                    existingField.Order = fieldDto.Order;

                    // Update options
                    existingField.Options = fieldDto.Options?.Select(o => new FormFieldOption
                    {
                        Label = o.Label,
                        Value = o.Value,
                        Name = o.Name
                    }).ToList();
                }
                else
                {
                    // Add new field
                    existingForm.Fields.Add(new FormField
                    {
                        Id = Guid.NewGuid().ToString(),
                        FieldType = fieldDto.FieldType,
                        Label = fieldDto.Label,
                        Value = fieldDto.Value,
                        IsRequired = fieldDto.IsRequired,
                        Placeholder = fieldDto.Placeholder,
                        Order = fieldDto.Order,
                        Options = fieldDto.Options?.Select(o => new FormFieldOption
                        {
                            Label = o.Label,
                            Value = o.Value,
                            Name = o.Name
                        }).ToList()
                    });
                }
            }

            // Remove deleted fields
            var fieldsToRemove = existingForm.Fields.Where(f => !formConfigDto.Fields.Select(fc => fc.Id).Contains(f.Id)).ToList();
            _context.FormFields.RemoveRange(fieldsToRemove);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Delete a form
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(string id)
        {
            var formConfig = await _context.FormConfigurations.FindAsync(id);
            if (formConfig == null)
                return NotFound();

            _context.FormConfigurations.Remove(formConfig);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Retrieve forms by user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFormsByUser(string userId)
        {
            var forms = await _context.FormConfigurations
                .Where(f => f.CreatedByUserId == userId)
                .Include(f => f.Fields)
                    .ThenInclude(f => f.Options)
                .ToListAsync();

            if (!forms.Any())
                return NotFound("No forms found for this user.");

            var formDtos = forms.Select(MapToFormConfigurationDto).ToList();
            return Ok(formDtos);
        }

        // Helper Method: Map FormConfiguration to FormConfigurationDto
        private static FormConfigurationDto MapToFormConfigurationDto(FormConfiguration formConfig)
        {
            return new FormConfigurationDto
            {
                Id = formConfig.Id,
                Name = formConfig.Name,
                CreatedByUserId = formConfig.CreatedByUserId,
                Fields = formConfig.Fields
                    .OrderBy(f => f.Order)
                    .Select(f => new FormFieldDto
                    {
                        Id = f.Id,
                        FieldType = f.FieldType,
                        Label = f.Label,
                        Value = f.Value,
                        IsRequired = f.IsRequired,
                        Placeholder = f.Placeholder,
                        Order = f.Order,
                        Options = f.Options?.Select(o => new FormFieldOptionDto
                        {
                            Label = o.Label,
                            Value = o.Value,
                            Name = o.Name
                        }).ToList()
                    }).ToList()
            };
        }
    }
}
