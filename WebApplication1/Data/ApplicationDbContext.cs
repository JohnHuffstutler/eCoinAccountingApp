using eCoinAccountingApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eCoinAccountingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Journal> Journals { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Journal>().ToTable("Journal");
        }

    }
}
