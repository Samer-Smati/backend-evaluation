using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PfeProject.Models;
using System.Reflection.Emit;


namespace PfeProject.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<EmployeeGroup> EmployeeGroups { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<ObjectiveEmployee> ObjectiveEmployees { get; set; }
        public DbSet<CampaignManager> CampaignManagers { get; set; }
        public DbSet<FormConfiguration> FormConfigurations { get; set; }
        public DbSet<FormField> FormFields { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Manager", NormalizedName = "MANAGER" },
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "HR", NormalizedName = "HR" },
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Employee", NormalizedName = "EMPLOYEE" }
            );
            builder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Id);  
            });
            builder.Entity<Campaign>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            builder.Entity<EmployeeGroup>()
             .HasKey(oe => oe.Id);
            builder.Entity<Campaign>()
           .HasKey(cm => cm.Id);
            builder.Entity<EmployeeGroup>()
                .HasOne(eg => eg.Group)
                .WithMany(g => g.EmployeeGroups)
                .HasForeignKey(eg => eg.GroupId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<EmployeeGroup>()
                .HasOne(eg => eg.Employee)
                .WithMany() 
                .HasForeignKey(eg => eg.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Objective>()
                .HasKey(o => o.Id);

            builder.Entity<Objective>()
                .HasOne(o => o.Campaign) 
                .WithMany(c => c.Objectives)
                .HasForeignKey(o => o.CampaignId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Objective>()
                .HasOne(o => o.CreatedByManager) 
                .WithMany() 
                .HasForeignKey(o => o.CreatedByManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ObjectiveEmployee>()
           .HasKey(oe => oe.Id);
            builder.Entity<ObjectiveEmployee>(entity =>
            {
                entity.HasKey(e => e.Id); 
            });

            builder.Entity<ObjectiveEmployee>()
                .HasOne(oe => oe.Objective)
                .WithMany(o => o.Employees)
                .HasForeignKey(oe => oe.ObjectiveId);

            builder.Entity<ObjectiveEmployee>()
                .HasOne(oe => oe.Employee)
                .WithMany()
                .HasForeignKey(oe => oe.EmployeeId);

            builder.Entity<CampaignManager>()
            .HasKey(cm => cm.Id);

            builder.Entity<CampaignManager>()
            .HasOne(cm => cm.Campaign)
            .WithMany(c => c.CampaignManagers)
            .HasForeignKey(cm => cm.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CampaignManager>()
            .HasOne(cm => cm.Manager)
            .WithMany()
            .HasForeignKey(cm => cm.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<FormConfiguration>()
           .HasKey(cm => cm.Id);
            builder.Entity<FormField>()
           .HasKey(cm => cm.Id);
            builder.Entity<FormConfiguration>()
           .HasMany(f => f.Fields)
           .WithOne()
           .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FormConfiguration>()
             .HasOne(f => f.CreatedByUser)
             .WithMany()
             .HasForeignKey(f => f.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
