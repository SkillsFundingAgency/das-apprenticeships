using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class ApprenticeshipsDataContext : DbContext
    {
        public ApprenticeshipsDataContext(DbContextOptions<ApprenticeshipsDataContext> options) : base(options)
        {
        }

        public virtual DbSet<Apprenticeship> Apprenticeships { get; set; }
        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<PriceHistory> PriceHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apprenticeship>()
                .HasMany(x => x.Approvals).WithOne().HasForeignKey(fk => fk.ApprenticeshipKey);
            modelBuilder.Entity<Apprenticeship>()
                .HasKey(a => new { a.Key });
            modelBuilder.Entity<Approval>()
                .Property(p => p.FundingType)
                .HasConversion(
                    v => v.ToString(),
                    v => (FundingType)Enum.Parse(typeof(FundingType), v));
            modelBuilder.Entity<PriceHistory>()
                .HasKey(x => x.Key);

            base.OnModelCreating(modelBuilder);
        }
    }
}
