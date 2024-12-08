using Bogus;
using PfeProject.Models;

namespace PfeProject.Utils
{
    public class ScriptCampaing
    {
        public List<Campaign> GenerateFakeCampaigns(int count)
        {
            var faker = new Faker<Campaign>()
                .RuleFor(c => c.Name, f => f.Lorem.Sentence(3))
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph())
                .RuleFor(c => c.StartDate, f => f.Date.Between(DateTime.Now.AddMonths(-12), DateTime.Now.AddMonths(12)))
                .RuleFor(c => c.EndDate, (f, c) => c.StartDate.AddMonths(f.Random.Int(1, 6)))
                .RuleFor(c => c.Type, f => f.PickRandom(new[] { "Trimestrial", "Annual", "Weekly" }))
                .RuleFor(c => c.CreatedByUserId, f => f.Random.Guid().ToString()); // Replace with valid user IDs

            return faker.Generate(count);
        }
    }
    
   
    }
