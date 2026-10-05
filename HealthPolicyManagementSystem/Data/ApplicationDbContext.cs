using Microsoft.EntityFrameworkCore;
using HealthPolicyManagementSystem.Models;

namespace HealthPolicyManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Policy> Policies { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public DbSet<Download> Downloads { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<PolicyVersion> PolicyVersions { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Template> Templates { get; set; }
    }
}
