namespace PfeProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Campaign
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; }

        [Required]
        [ForeignKey("User")]
        public string CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public ICollection<Objective> Objectives { get; set; }
        public List<CampaignManager> CampaignManagers { get; set; }
    }

}
