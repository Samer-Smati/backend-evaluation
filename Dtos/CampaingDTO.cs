namespace PfeProject.Dtos
{
    public class CampaignDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public string CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public List<UserCampaingDto> Managers { get; set; }
    }

    public class CampaignFilter
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? CreatedById { get; set; }
        public string? StartDate { get; set; } 
        public string? EndDate { get; set; } 
    }

    public class ShareCampaignDTO
    {
        public string CampaignId { get; set; }
        public List<string> ManagerIds { get; set; }
    }

    public class GetCampaignDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedBy { get; set; }

        public List<UserCampaingDto> Managers { get; set; }
    }

    public class ReportDTO
    {
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
        public int TotalCampaigns { get; set; }
        public int TotalTasks { get; set; }
        public int TasksOnTime { get; set; }
        public int TasksLate { get; set; }
        public int TasksNotStarted { get; set; }
    }

    public class GlobalReportDTO
    {
        public int TotalCampaigns { get; set; }
        public int RunningCampaigns { get; set; }
        public int NotStartedCampaigns { get; set; }
        public int DeliveredOnTimeCampaigns { get; set; }
    }
}
