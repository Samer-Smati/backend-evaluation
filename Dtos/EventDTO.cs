namespace PfeProject.Dtos
{
    public class EventDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ManagerId { get; set; }
        public string EmployeeId { get; set; }
    }

    public class CreateEventDto
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ManagerId { get; set; }
        public string EmployeeId { get; set; }
    }

    public class UpdateEventDto
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EmployeeId { get; set; }
    }

    public class EventReturnDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EventUserDto Manager { get; set; }
        public EventUserDto Employee { get; set; }
    }

    public class EventUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; }
    }


}
