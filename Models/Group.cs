namespace PfeProject.Models
{
    public class Group
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public User Manager { get; set; }
        public ICollection<EmployeeGroup> EmployeeGroups { get; set; }
    }

}
