using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PfeProject.Data;
using PfeProject.Dtos;
using PfeProject.Models;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    // Get report by manager
    [HttpGet("by-manager/{managerId}")]
    public async Task<IActionResult> GetReportByManager(string managerId)
    {
        var campaigns = await _context.Campaigns
            .Where(c => c.CreatedByUserId == managerId)
            .Include(c => c.Objectives)
            .ThenInclude(o => o.Employees)
            .ToListAsync();

        var totalCampaigns = campaigns.Count;
        var totalTasks = campaigns.SelectMany(c => c.Objectives.SelectMany(o => o.Employees)).Count();
        var tasksOnTime = campaigns.SelectMany(c => c.Objectives)
            .Count(o => o.Status == ObjectiveStatus.Done && o.EndDate <= o.DueDate);
        var tasksLate = campaigns.SelectMany(c => c.Objectives)
            .Count(o => o.Status == ObjectiveStatus.Done && o.EndDate > o.DueDate);
        var tasksNotStarted = campaigns.SelectMany(c => c.Objectives)
            .Count(o => o.Status == ObjectiveStatus.New); 

        var managerName = (await _context.Users.FindAsync(managerId))?.UserName;

        var report = new ReportDTO
        {
            ManagerId = managerId,
            ManagerName = managerName??"",
            TotalCampaigns = totalCampaigns,
            TotalTasks = totalTasks,
            TasksOnTime = tasksOnTime,
            TasksLate = tasksLate,
            TasksNotStarted = tasksNotStarted
        };

        return Ok(report);
    }

    // Get report by campaign
    [HttpGet("by-campaign/{campaignId}")]
    public async Task<IActionResult> GetReportByCampaign(string campaignId)
    {
        var campaign = await _context.Campaigns
            .Include(c => c.Objectives)
            .ThenInclude(o => o.Employees)
            .FirstOrDefaultAsync(c => c.Id == campaignId);

        if (campaign == null)
            return NotFound("Campaign not found.");

        var totalTasks = campaign.Objectives.SelectMany(o => o.Employees).Count();
        var tasksOnTime = campaign.Objectives
            .Count(o => o.Status == ObjectiveStatus.Done && o.EndDate <= o.DueDate);
        var tasksLate = campaign.Objectives
            .Count(o => o.Status == ObjectiveStatus.Done && o.EndDate > o.DueDate);
        var tasksNotStarted = campaign.Objectives
            .Count(o => o.Status == ObjectiveStatus.New);

        var report = new ReportDTO
        {
            ManagerId = campaign.CreatedByUserId,
            ManagerName = (await _context.Users.FindAsync(campaign.CreatedByUserId))?.UserName ?? "",
            TotalCampaigns = 1,
            TotalTasks = totalTasks,
            TasksOnTime = tasksOnTime,
            TasksLate = tasksLate,
            TasksNotStarted = tasksNotStarted 
        };

        return Ok(report);
    }

    // Get report of all campaigns
    [HttpGet]
    public async Task<IActionResult> GetAllReports()
    {
        var reports = await _context.Campaigns
            .Select(c => new ReportDTO
            {
                ManagerId = c.CreatedByUserId,
                ManagerName = _context.Users.Find(c.CreatedByUserId ?? "").UserName ?? "",
                TotalCampaigns = 1,
                TotalTasks = c.Objectives.SelectMany(o => o.Employees).Count(),
                TasksOnTime = c.Objectives.Count(o => o.Status == ObjectiveStatus.Done && o.EndDate <= o.DueDate),
                TasksLate = c.Objectives.Count(o => o.Status == ObjectiveStatus.Done && o.EndDate > o.DueDate),
                TasksNotStarted = c.Objectives.Count(o => o.Status == ObjectiveStatus.New) // Count for not started tasks
            })
            .ToListAsync();

        return Ok(reports);
    }

    [HttpGet("global")]
    public async Task<IActionResult> GetGlobalCampaignReport()
    {
        var campaigns = await _context.Campaigns
            .Include(c => c.Objectives)
            .ToListAsync();

        var totalCampaigns = campaigns.Count;
        var runningCampaigns = campaigns.Count(c => c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now);
        var notStartedCampaigns = campaigns.Count(c => c.StartDate > DateTime.Now);
        var deliveredOnTimeCampaigns = campaigns.Count(c => c.EndDate <= DateTime.Now &&
            c.Objectives.All(o => o.Status == ObjectiveStatus.Done && o.EndDate <= o.DueDate));

        var report = new GlobalReportDTO
        {
            TotalCampaigns = totalCampaigns,
            RunningCampaigns = runningCampaigns,
            NotStartedCampaigns = notStartedCampaigns,
            DeliveredOnTimeCampaigns = deliveredOnTimeCampaigns
        };

        return Ok(report);
    }

    // Get campaigns created per timeframe (day, month, year)
    [HttpGet("created-per/{timeframe}")]
    public async Task<IActionResult> GetCampaignsCreatedPerTimeframe(string timeframe)
    {
        var campaigns = await _context.Campaigns.ToListAsync();

        // Calculate date thresholds
        var now = DateTime.Now;
        var last30Days = now.AddDays(-30);
        var last12Months = now.AddMonths(-12);

        var report = timeframe.ToLower() switch
        {
            "day" => campaigns
                .Where(c => c.CreatedDate >= last30Days) // Filter for the last 30 days
                .GroupBy(c => c.CreatedDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                }),
            "month" => campaigns
                .Where(c => c.CreatedDate >= last12Months) // Filter for the last 12 months
                .GroupBy(c => new { Year = c.CreatedDate.Year, Month = c.CreatedDate.Month })
                .Select(g => new
                {
                    Date = $"{g.Key.Year}-{g.Key.Month:00}", // Format as "YYYY-MM"
                    Count = g.Count()
                }),
            "year" => campaigns
                .GroupBy(c => c.CreatedDate.Year) // No filtering, include all years
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                }),
            _ => Enumerable.Empty<object>(),
        };

        return Ok(report);

    }

    // Get report of tasks with average tasks per campaign
    [HttpGet("tasks-report")]
    public async Task<IActionResult> GetTasksReport()
    {
        // Total tasks
        var totalTasks = await _context.Objectives
            .SelectMany(o => o.Employees) // Assuming ObjectiveEmployee is the table for tasks
            .CountAsync();

        // Tasks done
        var tasksDone = await _context.Objectives
            .Where(o => o.Status == ObjectiveStatus.Done)
            .SelectMany(o => o.Employees)
            .CountAsync();

        // New tasks
        var newTasks = await _context.Objectives
            .Where(o => o.Status == ObjectiveStatus.New)
            .SelectMany(o => o.Employees)
            .CountAsync();

        // Total campaigns
        var totalCampaigns = await _context.Campaigns.CountAsync();

        // Calculate average tasks per campaign
        var averageTasksPerCampaign = totalCampaigns > 0 ? (double)totalTasks / totalCampaigns : 0;

        return Ok(new
        {
            TotalTasks = totalTasks,
            TasksDone = tasksDone,
            NewTasks = newTasks,
            AverageTasksPerCampaign = averageTasksPerCampaign
        });
    }


}
