namespace PfeProject.Models
{
    public class ObjectiveEmployee
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ObjectiveId { get; set; }
        public Objective Objective { get; set; }

        public string EmployeeId { get; set; }
        public User Employee { get; set; }
    }

}
