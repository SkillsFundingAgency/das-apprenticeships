﻿using System.Collections.ObjectModel;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class ApprenticeshipDomainModel : AggregateRoot
    {
        private readonly DataAccess.Entities.Apprenticeship.Apprenticeship _entity;
        private readonly List<ApprovalDomainModel> _approvals;
        private readonly List<PriceHistoryDomainModel> _priceHistories;

        public Guid Key => _entity.Key;
        public string TrainingCode => _entity.TrainingCode;
        public string Uln => _entity.Uln;
        public string FirstName => _entity.FirstName;
        public string LastName => _entity.LastName;
        public DateTime DateOfBirth => _entity.DateOfBirth;
        public IReadOnlyCollection<ApprovalDomainModel> Approvals => new ReadOnlyCollection<ApprovalDomainModel>(_approvals);
        public IReadOnlyCollection<PriceHistoryDomainModel> PriceHistories => new ReadOnlyCollection<PriceHistoryDomainModel>(_priceHistories);

        public int? AgeAtStartOfApprenticeship
        {
            get
            {
                var firstApproval = _approvals.OrderBy(x => x.ActualStartDate).First();
                if (!firstApproval.ActualStartDate.HasValue)
                {
                    return null;
                }
                var age = firstApproval.ActualStartDate.Value.Year - DateOfBirth.Year;
                if (firstApproval.ActualStartDate.Value.Date.AddYears(-age) < DateOfBirth.Date)
                {
                    age--;
                }

                return age;
            }
        }

        internal static ApprenticeshipDomainModel New(string uln, string trainingCode, DateTime dateOfBirth, string firstName, string lastName, decimal? trainingPrice, decimal? endpointAssessmentPrice, decimal totalPrice, string apprenticeshipHashedId, int fundingBandMaximum)
        {
            return new ApprenticeshipDomainModel(new DataAccess.Entities.Apprenticeship.Apprenticeship());
        }

        internal static ApprenticeshipDomainModel Get(DataAccess.Entities.Apprenticeship.Apprenticeship entity)
        {
            return new ApprenticeshipDomainModel(entity);
        }

        private ApprenticeshipDomainModel(DataAccess.Entities.Apprenticeship.Apprenticeship entity)
        {
            _entity = entity;
            _approvals = entity.Approvals.Select(ApprovalDomainModel.Get).ToList();
            _priceHistories = entity.PriceHistories.Select(PriceHistoryDomainModel.Get).ToList();
            AddEvent(new ApprenticeshipCreated(_entity.Key));
        }

        public void AddApproval(long approvalsApprenticeshipId, long ukprn, long employerAccountId, string legalEntityName, DateTime? actualStartDate, DateTime plannedEndDate, decimal agreedPrice, long? fundingEmployerAccountId, FundingType fundingType, int fundingBandMaximum, DateTime? plannedStartDate, FundingPlatform? fundingPlatform)
        {
            var approval = ApprovalDomainModel.New(approvalsApprenticeshipId, ukprn, employerAccountId, legalEntityName, actualStartDate, plannedEndDate, agreedPrice, fundingEmployerAccountId, fundingType, fundingBandMaximum, plannedStartDate, fundingPlatform);
            _approvals.Add(approval);
            _entity.Approvals.Add(approval.GetEntity());
        }

        public DataAccess.Entities.Apprenticeship.Apprenticeship GetEntity()
        {
            return _entity;
        }

        public void AddPriceHistory(
            decimal? trainingPrice,
            decimal? assessmentPrice,
            decimal totalPrice,
            DateTime effectiveFromDate,
            DateTime createdDate,
            PriceChangeRequestStatus? priceChangeRequestStatus)
        {
            var priceHistory = PriceHistoryDomainModel.New(this.Key,
                trainingPrice,
                assessmentPrice,
                totalPrice,
                effectiveFromDate,
                createdDate,
                priceChangeRequestStatus);
            
            _priceHistories.Add(priceHistory);
            _entity.PriceHistories.Add(priceHistory.GetModel());
        }
    }
}