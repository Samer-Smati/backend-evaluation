using System.ComponentModel.DataAnnotations;

namespace PfeProject.Models
{
    using System;
    using System.Collections.Generic;

    public class FormField
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FieldType { get; set; } = string.Empty; // e.g., "text", "number", "textarea", "checkbox", "radio"
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // Unique name for the field
        public string? Value { get; set; } // Nullable to handle cases like `number` or `checkbox`
        public bool IsRequired { get; set; } = false;
        public string Placeholder { get; set; } = string.Empty;
        public int Order { get; set; } = 0;

        public List<FormFieldOption>? Options { get; set; }
    }

    public class FormFieldOption
    {
        public string Label { get; set; } = string.Empty; 
        public string? Value { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FormFieldId { get; set; } = string.Empty;
    }


}
