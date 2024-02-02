using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class ApprenticeshipsDataContext : DbContext
    {
        private readonly IAccountIdClaimsHandler _accountIdClaimsHandler;
        private readonly AccountIdClaims _accountIdClaims;
        public ApprenticeshipsDataContext(
            DbContextOptions<ApprenticeshipsDataContext> options, IAccountIdClaimsHandler accountIdClaimsHandler) : base(options)
        {
            _accountIdClaimsHandler = accountIdClaimsHandler;
            _accountIdClaims = _accountIdClaimsHandler.GetAccountIdClaims();
        }

        public virtual DbSet<Apprenticeship> Apprenticeships { get; set; }
        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<PriceHistory> PriceHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apprenticeship>()
                .HasKey(a => new { a.Key });

            // Approval
            modelBuilder.Entity<Apprenticeship>()
                .HasMany(x => x.Approvals).WithOne()
                .HasForeignKey(fk => fk.ApprenticeshipKey);
            modelBuilder.Entity<Approval>()
                .Property(p => p.FundingType)
                .HasConversion(
                    v => v.ToString(),
                    v => (FundingType)Enum.Parse(typeof(FundingType), v));

            // PriceHistory
            modelBuilder.Entity<PriceHistory>()
                .HasKey(x => x.Key);
            modelBuilder.Entity<PriceHistory>()
                .Property(x => x.PriceChangeRequestStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (PriceChangeRequestStatus)Enum.Parse(typeof(PriceChangeRequestStatus), v));

            // Global query filters
            if (_accountIdClaims.IsClaimsValidationRequired)
            {
                ApplyFiltersOnAccountId(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }

        private void ApplyFiltersOnAccountId(ModelBuilder modelBuilder)
        {
            switch (_accountIdClaims.AccountIdClaimsType)
            {
                case AccountIdClaimsType.Provider:
                    modelBuilder.Entity<Apprenticeship>()
                        .HasQueryFilter(x => x.UKPRN == _accountIdClaims.AccountId);
                    modelBuilder.Entity<PriceHistory>()
                        .HasQueryFilter(x => x.Apprenticeship.UKPRN == _accountIdClaims.AccountId);
                    modelBuilder.Entity<Approval>()
                        .HasQueryFilter(x => x.Apprenticeship.UKPRN == _accountIdClaims.AccountId);
                    break;
                case AccountIdClaimsType.Employer:
                    modelBuilder.Entity<Apprenticeship>()
                        .HasQueryFilter(x => x.EmployerAccountId == _accountIdClaims.AccountId);
                    modelBuilder.Entity<PriceHistory>()
                        .HasQueryFilter(x => x.Apprenticeship.EmployerAccountId == _accountIdClaims.AccountId);
                    modelBuilder.Entity<Approval>()
                        .HasQueryFilter(x => x.Apprenticeship.EmployerAccountId == _accountIdClaims.AccountId);
                    break;
            }
        }
    }
}
