﻿using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.AcceptanceTests.Bindings
{
    [Binding]
    public static class DatabasePerTestRunHook
    {
        [BeforeTestRun(Order = 1)]
        public static void RefreshDatabaseModel()
        {
            SqlDatabaseModel.Update("SFA.DAS.Apprenticeships.Database");
        }
    }
}
