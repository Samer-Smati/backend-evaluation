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
        public string Id { get; set; }
        public string FieldType { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool IsRequired { get; set; }
        public string Placeholder { get; set; }
        public int Order { get; set; }
    }

}
