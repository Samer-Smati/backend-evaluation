namespace PfeProject.Dtos
{
    public class ReportDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string CreatedById { get; set; }
    }

}
