using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class Approval
    {
        private readonly ApprovalModel _model;

        public long ApprovalsApprenticeshipId => _model.ApprovalsApprenticeshipId;
        public long Ukprn => _model.UKPRN;
        public long EmployerAccountId => _model.EmployerAccountId;
        public string LegalEntityName => _model.LegalEntityName;
        public DateTime ActualStartDate => _model.ActualStartDate;
        public DateTime PlannedEndDate => _model.PlannedEndDate;
        public decimal AgreedPrice => _model.AgreedPrice;
        public long FunctionEmployerAccountId => _model.FundingEmployerAccountId;
        public FundingType FundingType => _model.FundingType;

        internal static Approval New(long approvalsApprenticeshipId, long ukprn, long employerAccountId, string legalEntityName, DateTime actualStartDate, DateTime plannedEndDate, decimal agreedPrice, long fundingEmployerAccountId, FundingType fundingType)
        {
            return new Approval(new ApprovalModel
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
                UKPRN = ukprn
            });
        }

        private Approval(ApprovalModel model)
        {
            _model = model;
        }

        public ApprovalModel GetModel()
        {
            return _model;
        }
    }
}
