using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.DataAccess
{
    public class ApprenticeshipsDataContext : DbContext
    {
        public ApprenticeshipsDataContext(DbContextOptions<ApprenticeshipsDataContext> options) : base(options)
        {
        }

        public virtual DbSet<Apprenticeship> Apprenticeships { get; set; }
        public virtual DbSet<Approval> Approvals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apprenticeship>().HasKey(a => new { a.Key });

            base.OnModelCreating(modelBuilder);
        }
    }
}
