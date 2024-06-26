﻿﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class ApprenticeshipsDataContext : DbContext
    {
        private readonly IAccountIdAuthorizer _accountIdAuthorizer;

        public ApprenticeshipsDataContext(DbContextOptions<ApprenticeshipsDataContext> options,
            IAccountIdAuthorizer accountIdAuthorizer) : base(options)
        {
            _accountIdAuthorizer = accountIdAuthorizer;
        }

        public IQueryable<Apprenticeship> Apprenticeships => _accountIdAuthorizer.ApplyAuthorizationFilterOnQueries(ApprenticeshipsDbSet);
        public virtual DbSet<Apprenticeship> ApprenticeshipsDbSet { get; set; }
        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<PriceHistory> PriceHistories { get; set; }
        public virtual DbSet<StartDateChange> StartDateChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apprenticeship
            modelBuilder.Entity<Apprenticeship>()
                .HasMany(x => x.Approvals).WithOne().HasForeignKey(fk => fk.ApprenticeshipKey);
            modelBuilder.Entity<Apprenticeship>()
                .HasKey(a => new { a.Key });
            
            // Approval
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
                    v => (ChangeRequestStatus)Enum.Parse(typeof(ChangeRequestStatus), v));
            modelBuilder.Entity<PriceHistory>()
                .Property(x => x.Initiator)
                .HasConversion(
                    v => v.ToString(),
                    v => (ChangeInitiator)Enum.Parse(typeof(ChangeInitiator), v));
            
            // StartDateChange
            modelBuilder.Entity<StartDateChange>()
                .HasKey(x => x.Key);
            modelBuilder.Entity<StartDateChange>()
                .Property(x => x.RequestStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (ChangeRequestStatus)Enum.Parse(typeof(ChangeRequestStatus), v));
            modelBuilder.Entity<StartDateChange>()
                .Property(x => x.Initiator)
                .HasConversion(
                    v => v.ToString(),
                    v => (ChangeInitiator)Enum.Parse(typeof(ChangeInitiator), v));

            base.OnModelCreating(modelBuilder);
        }
    }
}