using System.ComponentModel.DataAnnotations;

namespace PfeProject.Models
{
    public class FormField
    {
        public string Id { get; set; }
        public string FieldType { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = false;
        public string Placeholder { get; set; } = string.Empty;
        public int Order { get; set; } = 0;
    }

}
