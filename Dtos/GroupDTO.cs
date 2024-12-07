namespace PfeProject.Dtos
{
    public class GroupDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
        public List<string> EmployeeIds { get; set; } = new List<string>();
    }

}
