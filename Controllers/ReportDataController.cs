using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PfeProject.Data;
using PfeProject.Dtos;
using PfeProject.Models;

namespace PfeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportDataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] ReportDto reportDto)
        {
            if (reportDto == null)
                return BadRequest("Invalid report data.");

            var report = new Report
            {
                Name = reportDto.Name,
                Description = reportDto.Description,
                CreatedDate = DateTime.UtcNow, 
                CreatedById = reportDto.CreatedById 
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, reportDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(string id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return NotFound();

            // Map to DTO
            var reportDto = new ReportDto
            {
                Id = report.Id,
                Name = report.Name,
                Description = report.Description,
                CreatedDate = report.CreatedDate,
                CreatedById = report.CreatedById
            };

            return Ok(reportDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetReports()
        {
            var reports = await _context.Reports.ToListAsync();

            // Map to DTOs
            var reportDtos = reports.Select(report => new ReportDto
            {
                Id = report.Id,
                Name = report.Name,
                Description = report.Description,
                CreatedDate = report.CreatedDate,
                CreatedById = report.CreatedById
            }).ToList();

            return Ok(reportDtos);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(string id, [FromBody] ReportDto reportDto)
        {
            if (id != reportDto.Id)
                return BadRequest("Report ID mismatch.");

            var existingReport = await _context.Reports.FindAsync(id);
            if (existingReport == null)
                return NotFound();

            existingReport.Name = reportDto.Name;
            existingReport.Description = reportDto.Description;
            // Do not update CreatedDate or CreatedById
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(string id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return NotFound();

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
