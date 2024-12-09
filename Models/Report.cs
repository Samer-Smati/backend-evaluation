namespace PfeProject.Models
{
    public class Report
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }  
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedById { get; set; }  
        public virtual User CreatedBy { get; set; }  
    }

}
