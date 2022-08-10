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
        public IReadOnlyCollection<Approval> Approvals => new ReadOnlyCollection<Approval>(_approvals);

        internal static Apprenticeship New(string uln, string trainingCode)
        {
            return new Apprenticeship(new ApprenticeshipModel { Key = Guid.NewGuid(), Uln = uln, TrainingCode = trainingCode });
        }

        internal static Apprenticeship Get(ApprenticeshipModel model)
        {
            return new Apprenticeship(model);
        }

        private Apprenticeship(ApprenticeshipModel model)
        {
            _model = model;
            _approvals = new List<Approval>();
            AddEvent(new ApprenticeshipCreated(_model.Key));
        }

        public void AddApproval(long approvalsApprenticeshipId, long ukprn, long employerAccountId, string legalEntityName, DateTime? actualStartDate, DateTime? plannedEndDate, decimal agreedPrice, long fundingEmployerAccountId, FundingType fundingType, decimal fundingBandMaximum)
        {
            var approval = Approval.New(approvalsApprenticeshipId, ukprn, employerAccountId, legalEntityName, actualStartDate, plannedEndDate, agreedPrice, fundingEmployerAccountId, fundingType, fundingBandMaximum);
            _approvals.Add(approval);
            _model.Approvals.Add(approval.GetModel());
        }

        public ApprenticeshipModel GetModel()
        {
            return _model;
        }
    }
}
