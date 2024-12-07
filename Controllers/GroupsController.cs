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
            var group = await _context.Groups
                .Include(g => g.EmployeeGroups)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
                return NotFound("Group not found.");

            group.Name = groupDto.Name;
            var existingEmployeeGroups = group.EmployeeGroups.ToList();
            var employeeGroupsToRemove = existingEmployeeGroups
                .Where(eg => !groupDto.EmployeeIds.Contains(eg.EmployeeId))
                .ToList();

            foreach (var employeeGroup in employeeGroupsToRemove)
            {
                group.EmployeeGroups.Remove(employeeGroup);
                _context.EmployeeGroups.Remove(employeeGroup);
            }

            if (groupDto.EmployeeIds != null && groupDto.EmployeeIds.Any())
            {
                var employeeGroupsToAdd = groupDto.EmployeeIds
                    .Where(empId => !existingEmployeeGroups.Any(eg => eg.EmployeeId == empId))
                    .Select(empId => new EmployeeGroup { GroupId = id, EmployeeId = empId })
                    .ToList();

                foreach (var employeeGroup in employeeGroupsToAdd)
                {
                    group.EmployeeGroups.Add(employeeGroup);
                }
            }


            _context.Groups.Update(group);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // Delete a Group
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound("Group not found.");

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Get Group by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var group = await _context.Groups
                .Include(g => g.Manager)
                .Include(g => g.EmployeeGroups)
                .ThenInclude(eg => eg.Employee)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (group == null)
                return NotFound("Group not found.");

            // Map to DTO
            var groupDto = new GroupObjectDto
            {
                Id = group.Id,
                Name = group.Name,
                ManagerId = group.ManagerId,
                ManagerName = group.Manager?.FirstName+" "+group.Manager?.LastName ?? "N/A",
                Employees = group.EmployeeGroups
                    .Select(eg => new EmployeeDto
                    {
                        EmployeeId = eg.EmployeeId,
                        EmployeeName = eg.Employee?.FirstName +" "+ eg.Employee?.LastName
                    })
                    .ToList()
            };

            return Ok(groupDto);
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

            var groupDtos = groups.Select(group => new GroupObjectDto
            {
                Id = group.Id,
                Name = group.Name,
                ManagerId = group.ManagerId,
                ManagerName = group.Manager?.FirstName + " " + group.Manager?.LastName ?? "N/A",
                Employees = group.EmployeeGroups
                    .Select(eg => new EmployeeDto
                    {
                        EmployeeId = eg.EmployeeId,
                        EmployeeName = eg.Employee?.FirstName + " " + eg.Employee?.LastName
                    })
                    .ToList()
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

            var groupDtos = groups.Select(group => new GroupObjectDto
            {
                Id = group.Id,
                Name = group.Name,
                ManagerId = group.ManagerId,
                ManagerName = group.Manager?.FirstName + " " + group.Manager?.LastName ?? "N/A",
                Employees = group.EmployeeGroups
                    .Select(eg => new EmployeeDto
                    {
                        EmployeeId = eg.EmployeeId,
                        EmployeeName = eg.Employee?.FirstName + " " + eg.Employee?.LastName
                    })
                    .ToList()
            }).ToList();

            return Ok(groupDtos);
        }

    }
}