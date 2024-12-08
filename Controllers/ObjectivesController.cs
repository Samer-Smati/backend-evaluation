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
    public class ObjectivesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ObjectivesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Objectives
        [HttpPost]
        public async Task<IActionResult> CreateObjective([FromBody] CreateObjectiveDTO dto)
        {
            // Validate Campaign
            var campaign = await _context.Campaigns.FindAsync(dto.CampaignId);
            if (campaign == null) return NotFound("Campaign not found.");

            // Validate Dates
            if (dto.StartDate > dto.DueDate) return BadRequest("Start date cannot be later than end date.");
           
            // Create Objective
            var objective = new Objective
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                DueDate = dto.DueDate,
                CampaignId = dto.CampaignId,
                CreatedByManagerId = dto.CreatedByManagerId,
                Employees = dto.EmployeeIds.Select(id => new ObjectiveEmployee { EmployeeId = id }).ToList()
            };

            _context.Objectives.Add(objective);
            await _context.SaveChangesAsync();

            return Ok(objective.Id);
        }

        // GET: api/Objectives
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetObjectiveDTO>>> GetAllObjectives()
        {
            var objectives = await _context.Objectives
                .Include(o => o.Campaign)
                .Include(o => o.Employees)
                    .ThenInclude(e => e.Employee)
                .ToListAsync();

            var result = objectives.Select(o => new GetObjectiveDTO
            {
                Id = o.Id,
                Title = o.Title,
                Description = o.Description,
                StartDate = o.StartDate,
                DueDate = o.DueDate,
                CampaignName = o.Campaign.Name,
                Employees = o.Employees.Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Employee.UserName
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/Objectives/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetObjectiveDTO>> GetObjectiveById(string id)
        {
            var objective = await _context.Objectives
                .Include(o => o.Campaign)
                .Include(o => o.Employees)
                    .ThenInclude(e => e.Employee)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (objective == null) return NotFound("Objective not found.");

            var result = new GetObjectiveDTO
            {
                Id = objective.Id,
                Title = objective.Title,
                Description = objective.Description,
                StartDate = objective.StartDate,
                EndDate = objective.EndDate,
                DueDate = objective.DueDate,
                CampaignName = objective.Campaign.Name,
                Status = objective.Status,
                Employees = objective.Employees.Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Employee.UserName
                }).ToList()
            };

            return Ok(result);
        }

        // PUT: api/Objectives/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateObjective(string id, [FromBody] CreateObjectiveDTO dto)
        {
            var objective = await _context.Objectives.Include(o => o.Employees).FirstOrDefaultAsync(o => o.Id == id);
            if (objective == null) return NotFound("Objective not found.");

            // Validate Campaign
            var campaign = await _context.Campaigns.FindAsync(dto.CampaignId);
            if (campaign == null) return NotFound("Campaign not found.");

            // Validate Dates
            if (dto.StartDate > dto.DueDate) return BadRequest("Start date cannot be later than end date.");
         
            // Update Objective
            objective.Title = dto.Title;
            objective.Description = dto.Description;
            objective.StartDate = dto.StartDate;
            objective.DueDate = dto.DueDate;
            objective.CampaignId = dto.CampaignId;
            objective.CreatedByManagerId = dto.CreatedByManagerId;
            objective.Status = ObjectiveStatus.New;
            // Update Employees
            objective.Employees.Clear();
            objective.Employees = dto.EmployeeIds.Select(id => new ObjectiveEmployee { EmployeeId = id }).ToList();

            _context.Objectives.Update(objective);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Objectives/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObjective(int id)
        {
            var objective = await _context.Objectives.FindAsync(id);
            if (objective == null) return NotFound("Objective not found.");

            _context.Objectives.Remove(objective);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Objectives/ByEmployee/{employeeId}
        [HttpGet("ByEmployee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<GetObjectiveDTO>>> GetObjectivesByEmployee(string employeeId)
        {
            var objectives = await _context.Objectives
                .Include(o => o.Campaign)
                .Include(o => o.Employees)
                    .ThenInclude(e => e.Employee)
                .Where(o => o.Employees.Any(e => e.EmployeeId == employeeId))
                .ToListAsync();

            var result = objectives.Select(o => new GetObjectiveDTO
            {
                Id = o.Id,
                Title = o.Title,
                Description = o.Description,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                DueDate = o.DueDate,
                CampaignName = o.Campaign.Name,
                Status = o.Status,
                Employees = o.Employees.Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Employee.UserName
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/Objectives/ByManager/{managerId}
        [HttpGet("ByManager/{managerId}")]
        public async Task<ActionResult<IEnumerable<GetObjectiveDTO>>> GetObjectivesByManager(string managerId)
        {
            var objectives = await _context.Objectives
                .Include(o => o.Campaign)
                .Include(o => o.Employees)
                    .ThenInclude(e => e.Employee)
                .Where(o => o.CreatedByManagerId == managerId)
                .ToListAsync();

            var result = objectives.Select(o => new GetObjectiveDTO
            {
                Id = o.Id,
                Title = o.Title,
                Description = o.Description,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                DueDate = o.DueDate,
                CampaignName = o.Campaign.Name,
                Status = o.Status,
                Employees = o.Employees.Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Employee.UserName
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/Objectives/ByName/{name}
        [HttpGet("ByName/{name}")]
        public async Task<ActionResult<IEnumerable<GetObjectiveDTO>>> GetObjectivesByName(string name)
        {
            var objectives = await _context.Objectives
                .Include(o => o.Campaign)
                .Include(o => o.Employees)
                    .ThenInclude(e => e.Employee)
                .Where(o => o.Title.Contains(name) || o.Description.Contains(name))
                .ToListAsync();

            var result = objectives.Select(o => new GetObjectiveDTO
            {
                Id = o.Id,
                Title = o.Title,
                Description = o.Description,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                DueDate = o.DueDate,
                CampaignName = o.Campaign.Name,
                Status= o.Status,
                Employees = o.Employees.Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Employee.UserName
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/Objectives/ByCampaign/{campaignId}
        [HttpGet("ByCampaign/{campaignId}")]
        public async Task<ActionResult<IEnumerable<GetObjectiveDTO>>> GetObjectivesByCampaign(string campaignId)
        {
            var objectives = await _context.Objectives
                .Include(o => o.Campaign)
                .Include(o => o.Employees)
                    .ThenInclude(e => e.Employee)
                .Where(o => o.CampaignId == campaignId)
                .ToListAsync();

            var result = objectives.Select(o => new GetObjectiveDTO
            {
                Id = o.Id,
                Title = o.Title,
                Description = o.Description,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                DueDate = o.DueDate,
                CampaignName = o.Campaign.Name,
                Status = o.Status,
                Employees = o.Employees.Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Employee.UserName
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpPut("{id}/MarkAsDone")]
        public async Task<IActionResult> MarkObjectiveAsDone(int id)
        {
            var objective = await _context.Objectives.FindAsync(id);
            if (objective == null) return NotFound("Objective not found.");

            // Update the status and set the end date
            objective.Status = ObjectiveStatus.Done;
            objective.EndDate = DateTime.UtcNow; // or any other logic for setting the end date
            _context.Objectives.Update(objective);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }


}
