using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using VacationManager.Models;

namespace VacationManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected async override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasOne(user => user.Team)
                .WithMany(team => team.Developers)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Team>()
                .HasOne(team => team.Leader);

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole("CEO"),
                new IdentityRole("Developer"),
                new IdentityRole("Team Lead"),
                new IdentityRole("Unassigned")
            );
        }

        public DbSet<Team> Teams { get; set; }
    }
}