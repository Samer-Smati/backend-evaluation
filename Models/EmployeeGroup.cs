using System.ComponentModel.DataAnnotations;

namespace PfeProject.Models
{
    public class EmployeeGroup
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public Group Group { get; set; }
        public string EmployeeId { get; set; }
        public User Employee { get; set; }
    }

}
