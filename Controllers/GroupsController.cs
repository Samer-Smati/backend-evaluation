using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PfeProject.Data;
using PfeProject.Dtos;
using PfeProject.Models;

namespace PfeProject.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public GroupsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Create a Group
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] GroupDto groupDto)
        {
            var manager = await _userManager.FindByIdAsync(groupDto.ManagerId);
            if (manager == null || !await _userManager.IsInRoleAsync(manager, "Manager"))
                return BadRequest("Invalid manager.");

            var group = new Group
            {
                Name = groupDto.Name,
                ManagerId = groupDto.ManagerId,
                EmployeeGroups = groupDto.EmployeeIds.Select(id => new EmployeeGroup { EmployeeId = id }).ToList()
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return Ok(new { GroupId = group.Id });
        }

        // Update a Group
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateGroup(string id, [FromBody] GroupDto groupDto)
        {
            var group = await _context.Groups.Include(g => g.EmployeeGroups).FirstOrDefaultAsync(g => g.Id == id);
            if (group == null)
                return NotFound("Group not found.");

            group.Name = groupDto.Name;

            // Update employees
            group.EmployeeGroups.Clear();
            group.EmployeeGroups = groupDto.EmployeeIds.Select(empId => new EmployeeGroup { GroupId = id, EmployeeId = empId }).ToList();

            await _context.SaveChangesAsync();
            return Ok();
        }

        // Delete a Group
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound("Group not found.");

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Get All Groups
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await _context.Groups
                .Include(g => g.Manager)
                .Include(g => g.EmployeeGroups)
                .ThenInclude(eg => eg.Employee)
                .ToListAsync();

            var groupDtos = groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                ManagerId = g.ManagerId,
                ManagerName = g.Manager?.UserName??"",
                EmployeeIds = g.EmployeeGroups.Select(eg => eg.EmployeeId).ToList()
            }).ToList();

            return Ok(groupDtos);
        }

        // Filter Groups
        [HttpGet("filter")]
        public async Task<IActionResult> FilterGroups([FromQuery] string? name, [FromQuery] string? managerId)
        {
            var query = _context.Groups.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(g => g.Name.Contains(name));
            if (!string.IsNullOrEmpty(managerId))
                query = query.Where(g => g.ManagerId == managerId);

            var groups = await query
                .Include(g => g.Manager)
                .Include(g => g.EmployeeGroups)
                .ThenInclude(eg => eg.Employee)
                .ToListAsync();

            var groupDtos = groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                ManagerId = g.ManagerId,
                ManagerName = g.Manager?.UserName??"",
                EmployeeIds = g.EmployeeGroups.Select(eg => eg.EmployeeId).ToList()
            }).ToList();

            return Ok(groupDtos);
        }
    }
}