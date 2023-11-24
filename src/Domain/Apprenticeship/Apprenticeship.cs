using System.Collections.ObjectModel;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class Apprenticeship : AggregateRoot
    {
        private readonly ApprenticeshipModel _model;
        private readonly List<Approval> _approvals;

        public Guid Key => _model.Key;
        public string TrainingCode => _model.TrainingCode;
        public string Uln => _model.Uln;
        public string FirstName => _model.FirstName;
        public string LastName => _model.LastName;
        public DateTime DateOfBirth => _model.DateOfBirth;
        public IReadOnlyCollection<Approval> Approvals => new ReadOnlyCollection<Approval>(_approvals);

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

        internal static Apprenticeship New(string uln, string trainingCode, DateTime dateOfBirth, string firstName,
            string lastName, decimal? trainingPrice, decimal? endpointAssessmentPrice, decimal totalPrice,
            string apprenticeshipHashedId, int fundingBandMaximum, DateTime? actualStartDate, DateTime plannedEndDate, long accountLegalEntityId)
        {
            return new Apprenticeship(new ApprenticeshipModel
            {
                Key = Guid.NewGuid(),
                Uln = uln,
                TrainingCode = trainingCode,
                DateOfBirth = dateOfBirth,
                FirstName = firstName,
                LastName = lastName,
                TrainingPrice = trainingPrice,
                EndPointAssessmentPrice = endpointAssessmentPrice,
                TotalPrice = totalPrice,
                ApprenticeshipHashedId = apprenticeshipHashedId,
                FundingBandMaximum = fundingBandMaximum,
                ActualStartDate = actualStartDate,
                PlannedEndDate = plannedEndDate,
                AccountLegalEntityId = accountLegalEntityId,
            });
        }

        internal static Apprenticeship Get(ApprenticeshipModel model)
        {
            return new Apprenticeship(model);
        }

        private Apprenticeship(ApprenticeshipModel model)
        {
            _model = model;
            _approvals = model.Approvals.Select(Approval.Get).ToList();
            AddEvent(new ApprenticeshipCreated(_model.Key));
        }

        public void AddApproval(long approvalsApprenticeshipId, long ukprn, long employerAccountId, string legalEntityName, DateTime? actualStartDate, DateTime plannedEndDate, decimal agreedPrice, long? fundingEmployerAccountId, FundingType fundingType, int fundingBandMaximum, DateTime? plannedStartDate, FundingPlatform? fundingPlatform)
        {
            var approval = Approval.New(approvalsApprenticeshipId, ukprn, employerAccountId, legalEntityName, actualStartDate, plannedEndDate, agreedPrice, fundingEmployerAccountId, fundingType, fundingBandMaximum, plannedStartDate, fundingPlatform);
            _approvals.Add(approval);
            _model.Approvals.Add(approval.GetModel());
        }

        public ApprenticeshipModel GetModel()
        {
            return _model;
        }
    }
}
