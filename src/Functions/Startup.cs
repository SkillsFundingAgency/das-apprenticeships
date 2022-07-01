using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using SFA.DAS.Apprenticeships.Functions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Apprenticeships.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //throw new NotImplementedException();
            //todo IoC etc.
        }
    }
}
