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

    public class GroupObjectDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; } // If you want the manager's name
        public List<EmployeeDto> Employees { get; set; }
    }

    public class EmployeeDto
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; } // Example: Add more properties if needed
    }

    public class UserCampaingDto
    {
        public string Id { get; set; }
        public string Name { get; set; } // Example: Add more properties if needed
    }


}
