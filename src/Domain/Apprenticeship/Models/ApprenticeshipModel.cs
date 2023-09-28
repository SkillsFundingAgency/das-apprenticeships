﻿namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models
{
    public class ApprenticeshipModel
    {
        public Guid Key { get; internal set; }
        public string Uln { get; set; }
        public string TrainingCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<ApprovalModel> Approvals { get; set; }
        public List<PriceHistoryModel> PriceHistory { get; set; }

        public ApprenticeshipModel()
        {
            Approvals = new List<ApprovalModel>();
            PriceHistory = new List<PriceHistoryModel>();
        }

        public ApprenticeshipModel(Guid key) : this()
        {
            Key = key;
        }
    }
}
