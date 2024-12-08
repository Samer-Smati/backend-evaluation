using PfeProject.Models;

namespace PfeProject.Dtos
{
    public class CreateObjectiveDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string CampaignId { get; set; }
        public string CreatedByManagerId { get; set; }
        public List<string> EmployeeIds { get; set; }
        public ObjectiveStatus Status { get; set; }
    }

    public class GetObjectiveDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime DueDate { get; set; }
        public string CampaignName { get; set; }
        public List<EmployeeDTO> Employees { get; set; }
        public ObjectiveStatus Status { get; set; }
    }

    public class EmployeeDTO
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
    }

}
