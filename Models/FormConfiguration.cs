

namespace PfeProject.Models
{
    public class FormConfiguration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<FormField> Fields { get; set; } = new List<FormField>();
    }
}
