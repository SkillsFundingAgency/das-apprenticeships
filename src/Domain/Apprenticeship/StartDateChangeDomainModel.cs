﻿using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class StartDateChangeDomainModel
    {
        private readonly StartDateChange _entity;

        public Guid Key => _entity.Key;
        public Guid ApprenticeshipKey => _entity.ApprenticeshipKey;
        public DateTime ActualStartDate => _entity.ActualStartDate;
        public string Reason => _entity.Reason;
        public string ProviderApprovedBy => _entity.ProviderApprovedBy;
        public DateTime? ProviderApprovedDate => _entity.ProviderApprovedDate;
        public string EmployerApprovedBy => _entity.EmployerApprovedBy;
        public DateTime? EmployerApprovedDate => _entity.EmployerApprovedDate;
        public DateTime CreatedDate => _entity.CreatedDate;
        public ChangeRequestStatus RequestStatus => _entity.RequestStatus;
        public ChangeInitiator? Initiator => _entity.Initiator;

        internal static StartDateChangeDomainModel New(Guid apprenticeshipKey,
            DateTime actualStartDate,
            string reason,
            string? providerApprovedBy,
            DateTime? providerApprovedDate,
            string? employerApprovedBy,
            DateTime? employerApprovedDate,
            DateTime createdDate,
            ChangeRequestStatus requestStatus,
            ChangeInitiator? initiator)
        {
            return new StartDateChangeDomainModel(new DataAccess.Entities.Apprenticeship.StartDateChange
            {
                ApprenticeshipKey = apprenticeshipKey,
                ActualStartDate = actualStartDate,
                Reason = reason,
                ProviderApprovedBy = providerApprovedBy,
                ProviderApprovedDate = providerApprovedDate,
                EmployerApprovedBy = employerApprovedBy,
                EmployerApprovedDate = employerApprovedDate,
                CreatedDate = createdDate,
                RequestStatus = requestStatus,
                Initiator = initiator
            });
        }

        private StartDateChangeDomainModel(StartDateChange entity)
        {
            _entity = entity;
        }

        public StartDateChange GetEntity()
        {
            return _entity;
        }

        internal static StartDateChangeDomainModel Get(StartDateChange entity)
        {
            return new StartDateChangeDomainModel(entity);
        }

        public void Approve(ApprovedBy approver, string? userId, DateTime approvedDate)
        {
            _entity.RequestStatus = ChangeRequestStatus.Approved;

            if (approver == ApprovedBy.Employer)
            {
                _entity.EmployerApprovedBy = userId;
                _entity.EmployerApprovedDate = approvedDate;
            }
            else
            {
                _entity.ProviderApprovedBy = userId;
                _entity.ProviderApprovedDate = approvedDate;
            }
        }
    }
}
