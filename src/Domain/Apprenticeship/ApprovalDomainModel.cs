using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class ApprovalDomainModel
    {
        private readonly DataAccess.Entities.Apprenticeship.Approval _entity;

        public long ApprovalsApprenticeshipId => _entity.ApprovalsApprenticeshipId;
        public long Ukprn => _entity.UKPRN;
        public long EmployerAccountId => _entity.EmployerAccountId;
        public string LegalEntityName => _entity.LegalEntityName;
        public DateTime? ActualStartDate => _entity.ActualStartDate;
        public DateTime PlannedEndDate => _entity.PlannedEndDate;
        public decimal AgreedPrice => _entity.AgreedPrice;
        public long? FundingEmployerAccountId => _entity.FundingEmployerAccountId;
        public FundingType FundingType => _entity.FundingType;
        public int FundingBandMaximum => _entity.FundingBandMaximum;
        public DateTime? PlannedStartDate => _entity.PlannedStartDate;
        public FundingPlatform? FundingPlatform => _entity.FundingPlatform;

        internal static ApprovalDomainModel New(long approvalsApprenticeshipId, long ukprn, long employerAccountId, string legalEntityName, DateTime? actualStartDate, DateTime plannedEndDate, decimal agreedPrice, long? fundingEmployerAccountId, FundingType fundingType, int fundingBandMaximum, DateTime? plannedStartDate, FundingPlatform? fundingPlatform)
        {
            return new ApprovalDomainModel(new DataAccess.Entities.Apprenticeship.Approval
            {
                ActualStartDate = actualStartDate, 
                AgreedPrice = agreedPrice,
                ApprovalsApprenticeshipId = approvalsApprenticeshipId, 
                EmployerAccountId = employerAccountId,
                FundingEmployerAccountId = fundingEmployerAccountId, 
                FundingType = fundingType, 
                Id = Guid.NewGuid(),
                LegalEntityName = legalEntityName,
                PlannedEndDate = plannedEndDate, 
                UKPRN = ukprn,
                FundingBandMaximum = fundingBandMaximum,
                PlannedStartDate = plannedStartDate,
                FundingPlatform = fundingPlatform,
                //ApprenticeshipKey = //todo map this?
            });
        }

        private ApprovalDomainModel(DataAccess.Entities.Apprenticeship.Approval entity)
        {
            _entity = entity;
        }

        public DataAccess.Entities.Apprenticeship.Approval GetEntity()
        {
            return _entity;
        }

        internal static ApprovalDomainModel Get(DataAccess.Entities.Apprenticeship.Approval approvalEntity)
        {
            return new ApprovalDomainModel(approvalEntity);
        }
    }
}
