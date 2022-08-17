using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship
{
    public class Approval
    {
        public Guid Id { get; set; }
        public Guid ApprenticeshipKey { get; set; }
        public long ApprovalsApprenticeshipId { get; set; }
        public long UKPRN { get; set; }
        public long EmployerAccountId { get; set; }
        public string LegalEntityName { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public decimal AgreedPrice { get; set; }
        public long FundingEmployerAccountId { get; set; }
        public FundingType FundingType { get; set; }
        public decimal FundingBandMaximum { get; set; }
    }
}
