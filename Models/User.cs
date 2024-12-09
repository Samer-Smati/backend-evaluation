using Microsoft.AspNetCore.Identity;

namespace PfeProject.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<FormSubmission> FormSubmissions { get; set; } = new List<FormSubmission>(); 
    }
}
