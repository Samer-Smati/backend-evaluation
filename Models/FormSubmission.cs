using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace PfeProject.Models
{
    public class FormSubmission
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FormId { get; set; }
        public FormConfiguration Form { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string FieldValuesJson { get; set; } // Store JSON as a string
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [NotMapped] // Exclude this property from the database schema
        public Dictionary<string, string> FieldValues
        {
            get => string.IsNullOrWhiteSpace(FieldValuesJson)
                ? new Dictionary<string, string>()
                : JsonConvert.DeserializeObject<Dictionary<string, string>>(FieldValuesJson);

            set => FieldValuesJson = JsonConvert.SerializeObject(value);
        }
    }


}
