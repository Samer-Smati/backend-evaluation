namespace PfeProject.Models
{
    public class Objective
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }

        // Dates
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime DueDate { get; set; }

        // Relationships
        public string CampaignId { get; set; }
        public Campaign Campaign { get; set; }

        public string CreatedByManagerId { get; set; }
        public User CreatedByManager { get; set; } // Assuming 'User' is the Identity user model

        public ICollection<ObjectiveEmployee> Employees { get; set; } // Many-to-Many relationship
        public ObjectiveStatus Status { get; set; }
    }
    public enum ObjectiveStatus
    {
        New,
        Done
    }
}
