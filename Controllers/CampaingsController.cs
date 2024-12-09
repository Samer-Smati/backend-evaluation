using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Fpe;
using PfeProject.Data;
using PfeProject.Dtos;
using PfeProject.Models;
using PfeProject.Utils;
using System.Globalization;

namespace PfeProject.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CampaignsController(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("seed-fake-campaigns-and-objectives")]
        public async Task<IActionResult> SeedFakeCampaignsAndObjectives()
        {
            // Retrieve users by roles
            var managerUsers = await _userManager.GetUsersInRoleAsync("Manager");
            var employeeUsers = await _userManager.GetUsersInRoleAsync("Employee");

            if (!managerUsers.Any() || !employeeUsers.Any())
            {
                return BadRequest("No valid Manager or Employee users found.");
            }

            var managerIds = managerUsers.Select(u => u.Id).ToList();
            var employeeIds = employeeUsers.Select(u => u.Id).ToList();

            // Seed campaigns
            var campaignFaker = new Faker<Campaign>()
                .RuleFor(c => c.Name, f => f.Lorem.Sentence(3))
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph())
                .RuleFor(c => c.StartDate, f => f.Date.Between(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(1)))
                .RuleFor(c => c.EndDate, (f, c) => c.StartDate.AddMonths(f.Random.Int(1, 6)))
                .RuleFor(c => c.Type, f => f.PickRandom(new[] { "Trimestrial", "Annual", "Weekly" }))
                .RuleFor(c => c.CreatedByUserId, f => f.PickRandom(managerIds))
                .RuleFor(c => c.CreatedDate, f => f.Date.Between(DateTime.Now.AddYears(-3), DateTime.Now));

            var fakeCampaigns = campaignFaker.Generate(200);
            await _context.Campaigns.AddRangeAsync(fakeCampaigns);
            await _context.SaveChangesAsync();

            // Get the saved campaigns
            var savedCampaigns = await _context.Campaigns.ToListAsync();

            // Seed objectives
            var objectiveFaker = new Faker<Objective>()
                .RuleFor(o => o.Title, f => f.Lorem.Sentence(2))
                .RuleFor(o => o.Description, f => f.Lorem.Paragraph())
                .RuleFor(o => o.StartDate, f => f.Date.Between(DateTime.Now.AddYears(-3), DateTime.Now))
                .RuleFor(o => o.DueDate, (f, o) => o.StartDate.AddDays(f.Random.Int(7, 30)))
                .RuleFor(o => o.CampaignId, f => f.PickRandom(savedCampaigns).Id)
                .RuleFor(o => o.CreatedByManagerId, f => f.PickRandom(managerIds));

            var fakeObjectives = objectiveFaker.Generate(500);

            // Seed ObjectiveEmployees
            var objectiveEmployeeFaker = new Faker<ObjectiveEmployee>()
                .RuleFor(oe => oe.ObjectiveId, f => f.PickRandom(fakeObjectives).Id)
                .RuleFor(oe => oe.EmployeeId, f => f.PickRandom(employeeIds))
                .RuleFor(oe => oe.Id, f => Guid.NewGuid().ToString())
                .RuleFor(oe => oe.Objective, (f, oe) => fakeObjectives.FirstOrDefault(o => o.Id == oe.ObjectiveId));

            var fakeObjectiveEmployees = objectiveEmployeeFaker.Generate(1000); // Assume 2 employees per objective on average
            await _context.ObjectiveEmployees.AddRangeAsync(fakeObjectiveEmployees);

            // Assign objectives to employees and set statuses
            foreach (var objEmp in fakeObjectiveEmployees)
            {
                if (new Random().Next(2) == 1) // 50% chance for the objective to be 'Done'
                {
                    objEmp.Objective.Status = ObjectiveStatus.Done; // Done
                    objEmp.Objective.EndDate = DateTime.Now.AddDays(new Random().Next(1, 30));
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "200 fake campaigns, 500 objectives, and 1000 objective-employee links seeded successfully!",
                CampaignCount = fakeCampaigns.Count,
                ObjectiveCount = fakeObjectives.Count,
                ObjectiveEmployeeCount = fakeObjectiveEmployees.Count
            });
        }




        // Create Campaign
        [HttpPost("create")]
        public async Task<IActionResult> CreateCampaign([FromBody] CampaignDto campaignDto)
        {
            if (!CampaignType.AllTypes.Contains(campaignDto.Type))
                return BadRequest("Invalid campaign type.");

            var user = await _userManager.FindByIdAsync(campaignDto.CreatedByUserId);
            if (user == null || !(await _userManager.IsInRoleAsync(user, "Manager") || await _userManager.IsInRoleAsync(user, "HR")))
                return BadRequest("Invalid creator.");

            var campaign = new Campaign
            {
                Name = campaignDto.Name,
                Description = campaignDto.Description,
                StartDate = campaignDto.StartDate,
                EndDate = campaignDto.EndDate,
                Type = campaignDto.Type,
                CreatedByUserId = campaignDto.CreatedByUserId
            };

            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();
            return Ok(new { CampaignId = campaign.Id });
        }

        // Update Campaign
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCampaign(string id, [FromBody] CampaignDto campaignDto)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
                return NotFound("Campaign not found.");

            if (!CampaignType.AllTypes.Contains(campaignDto.Type))
                return BadRequest("Invalid campaign type.");

            campaign.Name = campaignDto.Name;
            campaign.Description = campaignDto.Description;
            campaign.StartDate = campaignDto.StartDate;
            campaign.EndDate = campaignDto.EndDate;
            campaign.Type = campaignDto.Type;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // Delete Campaign
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCampaign(string id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
                return NotFound("Campaign not found.");

            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Get All Campaigns
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCampaigns()
        {
            var campaigns = await _context.Campaigns
                .Include(c => c.CreatedByUser)
                .Include(c=>c.CampaignManagers)
                .ToListAsync();

            var campaignDtos = campaigns.Select(c => new CampaignDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Type = c.Type,
                CreatedByUserId = c.CreatedByUserId,
                CreatedByUserName = c.CreatedByUser?.UserName??"",
                Managers = c.CampaignManagers
                    .Select(eg => new UserCampaingDto
                    {
                        Id = eg.ManagerId,
                        Name = eg.Manager?.FirstName + " " + eg.Manager?.LastName
                    })
                    .ToList()
            }).ToList();

            return Ok(campaignDtos);
        }

        // Filter Campaigns
        [HttpPost("filter")]
        public async Task<IActionResult> FilterCampaigns([FromBody] CampaignFilter filter)
        {
            var query = _context.Campaigns.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(c => c.Name.Contains(filter.Name));
            if (!string.IsNullOrEmpty(filter.Type))
                query = query.Where(c => c.Type == filter.Type);
            if (!string.IsNullOrEmpty(filter.CreatedById))
                query = query.Where(c => c.CreatedByUserId == filter.CreatedById);

            if (!string.IsNullOrEmpty(filter.StartDate) && DateTime.TryParseExact(filter.StartDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                query = query.Where(c => c.StartDate >= startDate);
            }

            if (!string.IsNullOrEmpty(filter.EndDate) && DateTime.TryParseExact(filter.EndDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                query = query.Where(c => c.EndDate <= endDate);
            }

            var campaigns = await query.Include(c => c.CreatedByUser).ToListAsync();

            var campaignDtos = campaigns.Select(c => new CampaignDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Type = c.Type,
                CreatedByUserId = c.CreatedByUserId,
                CreatedByUserName = c.CreatedByUser?.UserName??"",
                Managers = c.CampaignManagers?
                    .Select(eg => new UserCampaingDto
                    {
                        Id = eg.ManagerId,
                        Name = eg.Manager?.FirstName + " " + eg.Manager?.LastName
                    })
                    .ToList()
            }).ToList();

            return Ok(campaignDtos);
        }

        [HttpPost("Share")]
        public async Task<IActionResult> ShareCampaign([FromBody] ShareCampaignDTO dto)
        {   EmailService emailService = new EmailService();
            var campaign = await _context.Campaigns.FindAsync(dto.CampaignId);
            if (campaign == null) return NotFound("Campaign not found.");

            // Remove existing shares first if any
            var existingShares = _context.CampaignManagers
                .Where(cm => cm.CampaignId == dto.CampaignId).ToList();

            _context.CampaignManagers.RemoveRange(existingShares);

            // Add new shares
            foreach (var managerId in dto.ManagerIds)
            {
                var campaignManager = new CampaignManager
                {
                    CampaignId = dto.CampaignId,
                    ManagerId = managerId
                };
                _context.CampaignManagers.Add(campaignManager);
                var manager = await _context.Users.FindAsync(managerId);
                if (manager != null)
                {
                    try
                    {
                        var subject = "New Campaign Shared with You";
                        var body = $"Hello {manager.FirstName+" "+manager.LastName},\n\nA new campaign has been shared with you. Please check your dashboard for details.\n\nBest regards,\nYour Team";
                        await emailService.SendEmailAsync(manager.Email??"", subject, body);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email to manager {managerId}: {ex.Message}");
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Campaigns/Unshare/{campaignId}
        [HttpDelete("Unshare/{campaignId}")]
        public async Task<IActionResult> UnshareCampaign(string campaignId, [FromBody] List<string> managerIds)
        {
            var campaign = await _context.Campaigns.FindAsync(campaignId);
            if (campaign == null) return NotFound("Campaign not found.");

            var sharesToRemove = await _context.CampaignManagers
                .Where(cm => cm.CampaignId == campaignId && managerIds.Contains(cm.ManagerId))
                .ToListAsync();

            _context.CampaignManagers.RemoveRange(sharesToRemove);
            await _context.SaveChangesAsync();

            return Ok(); 
        }

        // GET: api/Campaigns/SharedWith/{managerId}
        [HttpGet("SharedWith/{managerId}")]
        public async Task<ActionResult<IEnumerable<GetCampaignDTO>>> GetSharedCampaigns(string managerId)
        {
            var campaigns = await _context.Campaigns
                .Include(c => c.CampaignManagers)
                    .ThenInclude(cm => cm.Manager)
                .Where(c => c.CampaignManagers.Any(cm => cm.ManagerId == managerId))
                .ToListAsync();

            var result = campaigns.Select(c => new GetCampaignDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                CreatedBy = c.CreatedByUser.UserName??"",
                Managers = c.CampaignManagers
                    .Select(eg => new UserCampaingDto
                    {
                        Id = eg.ManagerId,
                        Name = eg.Manager?.FirstName + " " + eg.Manager?.LastName
                    })
                    .ToList()
            }).ToList();

            return Ok(result);
        }

    }
}