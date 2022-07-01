using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.Acceptance
{
    public class TestContext : IDisposable
    {
        public TestFunction TestFunction { get; set; }
        public SqlDatabase SqlDatabase { get; set; }
        public void Dispose()
        {
            //throw new NotImplementedException(); //todo stop function?
        }
    }
}
