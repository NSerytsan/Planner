using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Planner.Data
{
    public class PlannerDbContext : IdentityDbContext
    {
        public PlannerDbContext(DbContextOptions<PlannerDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Plan> Plans { get; set; } = null!;
        public virtual DbSet<Event> Events { get; set; } = null!;
    }
}