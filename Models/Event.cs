namespace PfeProject.Models
{
    public class Event
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ManagerId { get; set; } // Foreign Key
        public string EmployeeId { get; set; } // Foreign Key

        public User Manager { get; set; } // Navigation Property
        public User Employee { get; set; } // Navigation Property
    }

}
