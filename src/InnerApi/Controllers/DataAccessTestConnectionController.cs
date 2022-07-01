using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.DataAccess;

namespace SFA.DAS.Apprenticeships.InnerApi.Controllers
{
    //todo this can be thrown away, but for now proves data access
    [ApiController]
    [Route("[controller]")]
    public class DataAccessTestConnectionController : ControllerBase
    {
        private readonly ILogger<DataAccessTestConnectionController> _logger;
        private readonly ApprenticeshipsDataContext _dataContext;

        public DataAccessTestConnectionController(ILogger<DataAccessTestConnectionController> logger, ApprenticeshipsDataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        [HttpGet(Name = "TestDBConnection")]
        public IEnumerable<long> GetUkprns()
        {
            return _dataContext.Earning.Select(x => x.Ukprn);
        }
    }
}