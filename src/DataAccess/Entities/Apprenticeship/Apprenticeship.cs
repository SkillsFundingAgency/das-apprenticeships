﻿namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship
{
    [Table("dbo.Apprenticeship")]
    [System.ComponentModel.DataAnnotations.Schema.Table("Apprenticeship")]
    public class Apprenticeship
    {
        [Key]
        public Guid Key { get; set; }
        public string Uln { get; set; }
        public string TrainingCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<Approval> Approvals { get; set; }
    }
}
