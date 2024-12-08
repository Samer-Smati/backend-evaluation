using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public CampaignsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("seed-fake-campaigns")]
        public async Task<IActionResult> SeedFakeCampaigns()
        {
            var validUserIds = await _userManager.Users
                .Select(u => u.Id)
                .ToListAsync();

            if (!validUserIds.Any())
                return BadRequest("No valid Manager or HR users found to assign campaigns.");

            // Generate 100 fake campaigns
            var fakeCampaigns = new List<Campaign>();
            var faker = new Faker<Campaign>()
                .RuleFor(c => c.Name, f => f.Lorem.Sentence(3))
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph())
                .RuleFor(c => c.StartDate, f => f.Date.Between(DateTime.Now.AddMonths(-12), DateTime.Now.AddMonths(12)))
                .RuleFor(c => c.EndDate, (f, c) => c.StartDate.AddMonths(f.Random.Int(1, 6)))
                .RuleFor(c => c.Type, f => f.PickRandom(new[] { "Trimestrial", "Annual", "Weekly" }))
                .RuleFor(c => c.CreatedByUserId, f => f.PickRandom(validUserIds))
                .RuleFor(c => c.CreatedDate, f => f.Date.Between(DateTime.Now.AddMonths(-24), DateTime.Now));

            fakeCampaigns = faker.Generate(100);

            // Add campaigns to the database
            await _context.Campaigns.AddRangeAsync(fakeCampaigns);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "100 fake campaigns seeded successfully!" });
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
        {
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