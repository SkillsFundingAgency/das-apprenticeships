using SFA.DAS.Apprenticeships.Queries.GetApprenticeships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice
{
    public class GetApprenticeshipPriceRequest : IQuery
    {
        public Guid ApprenticeshipKey { get; set; }
    }
}
