using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.DataTransferObjects
{
    [ExcludeFromCodeCoverage]
    public class Apprenticeship
    {
        public string Uln { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}