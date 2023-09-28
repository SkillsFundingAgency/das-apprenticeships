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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apprenticeship>()
                .HasKey(a => new { a.Key });

            modelBuilder.Entity<Apprenticeship>()
                .HasMany(x => x.Approvals).WithOne().HasForeignKey(fk => fk.ApprenticeshipKey);

            modelBuilder.Entity<Apprenticeship>()
                .OwnsMany(
                    a => a.PriceHistory,
                    e =>
                    {
                        e.HasKey(ph => new { ph.ApprenticeshipKey, ph.ApprovedDate, ph.EffectiveFrom });
                        e.WithOwner().HasForeignKey(fk => fk.ApprenticeshipKey);
                        e.HasIndex(a => a.ApprenticeshipKey);
                    });

            modelBuilder.Entity<Approval>()
                .Property(p => p.FundingType)
                .HasConversion(
                    v => v.ToString(),
                    v => (FundingType)Enum.Parse(typeof(FundingType), v));

            modelBuilder.Entity<Approval>()
                .HasIndex(a => a.ApprenticeshipKey);

            modelBuilder.Entity<Approval>()
                .HasIndex(a => a.ApprovalsApprenticeshipId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
