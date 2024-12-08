namespace PfeProject.Dtos
{
    public class FormConfigurationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CreatedByUserId { get; set; }
        public List<FormFieldDto> Fields { get; set; } = new List<FormFieldDto>();
    }

    public class FormFieldDto
    {
        public string Id { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = false;
        public string Placeholder { get; set; } = string.Empty;
        public int Order { get; set; } = 0;
        public List<FormFieldOptionDto> Options { get; set; } = new List<FormFieldOptionDto>(); // Added support for options
    }

    public class FormFieldOptionDto
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }


}
