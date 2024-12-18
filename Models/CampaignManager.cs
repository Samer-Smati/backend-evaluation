﻿namespace PfeProject.Models
{
    public class CampaignManager
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CampaignId { get; set; }
        public Campaign Campaign { get; set; }

        public string ManagerId { get; set; }
        public User Manager { get; set; }
    }

}
