namespace PfeProject.Models
{
    public class ObjectiveEmployee
    {
        public string Id { get; set; }
        public string ObjectiveId { get; set; }
        public Objective Objective { get; set; }

        public string EmployeeId { get; set; }
        public User Employee { get; set; }
    }

}
