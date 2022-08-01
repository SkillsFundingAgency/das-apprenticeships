using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.DataAccess.Mappers.Apprenticeship
{
    internal static class ApprovalsMapper
    {
        internal static List<Approval> Map(this List<ApprovalModel> domainModels, Guid apprenticeshipKey)
        {
            var dataModels = domainModels.Select(x => new Approval
            {
                ActualStartDate = x.ActualStartDate, 
                AgreedPrice = x.AgreedPrice, 
                ApprenticeshipKey = apprenticeshipKey,
                ApprovalsApprenticeshipId = x.ApprovalsApprenticeshipId, 
                EmployerAccountId = x.EmployerAccountId,
                FundingEmployerAccountId = x.FundingEmployerAccountId, 
                FundingType = x.FundingType, 
                Id = x.Id,
                LegalEntityName = x.LegalEntityName, 
                PlannedEndDate = x.PlannedEndDate, 
                UKPRN = x.UKPRN
            }).ToList();

            return dataModels;
        }

        internal static List<ApprovalModel> Map(this List<Approval> dataModels)
        {
            var domainModels = dataModels.Select(x => new ApprovalModel
            {
                Id = x.Id,
                FundingEmployerAccountId = x.FundingEmployerAccountId,
                EmployerAccountId = x.EmployerAccountId,
                ActualStartDate = x.ActualStartDate,
                AgreedPrice = x.AgreedPrice,
                ApprovalsApprenticeshipId = x.ApprovalsApprenticeshipId,
                FundingType = x.FundingType,
                LegalEntityName = x.LegalEntityName,
                PlannedEndDate = x.PlannedEndDate,
                UKPRN = x.UKPRN
            }).ToList();

            return domainModels;
        }
    }
}
