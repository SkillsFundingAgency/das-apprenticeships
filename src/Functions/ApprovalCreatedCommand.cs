﻿using System;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Functions
{
    /// <summary>
    /// Get this from the event handlers repo
    /// </summary>
    public class ApprovalCreatedCommand
    {
        public long Uln { get; set; }
        public string TrainingCode { get; set; }
        public long ApprovalsApprenticeshipId { get; set; }
        public long UKPRN { get; set; }
        public long EmployerAccountId { get; set; }
        public string LegalEntityName { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public decimal AgreedPrice { get; set; }
        public long FundingEmployerAccountId { get; set; }
        public FundingType FundingType { get; set; }
    }
}
