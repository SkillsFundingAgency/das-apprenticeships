﻿using System.Diagnostics.CodeAnalysis;
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
        public virtual DbSet<Episode> Episodes { get; set; }
        public virtual DbSet<EpisodePrice> EpisodePrices { get; set; }
        public virtual DbSet<PriceHistory> PriceHistories { get; set; }
        public virtual DbSet<StartDateChange> StartDateChanges { get; set; }
        public virtual DbSet<FreezeRequest> FreezeRequests { get; set; }
        public virtual DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apprenticeship
            modelBuilder.Entity<Apprenticeship>()
                .HasMany(x => x.Episodes)
                .WithOne()
                .HasForeignKey(fk => fk.ApprenticeshipKey);
            modelBuilder.Entity<Apprenticeship>()
                .HasKey(a => new { a.Key });

            // Episode
            modelBuilder.Entity<Episode>()
                .HasKey(a => new { a.Key });
            modelBuilder.Entity<Episode>()
                .Property(p => p.FundingType)
                .HasConversion(
                    v => v.ToString(),
                    v => (FundingType)Enum.Parse(typeof(FundingType), v));
            modelBuilder.Entity<Episode>()
                .Property(p => p.FundingPlatform)
                .HasConversion(
                    v => (int?)v,
                    v => (FundingPlatform?)v);

            // EpisodePrice
            modelBuilder.Entity<EpisodePrice>()
                .HasKey(x => x.Key);

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

            // FreezeRequest
            modelBuilder.Entity<FreezeRequest>()
                .HasKey(x => x.Key);

            // WithdrawalRequest
            modelBuilder.Entity<WithdrawalRequest>()
                .HasKey(x => x.Key);

            base.OnModelCreating(modelBuilder);
        }
    }
}