namespace SFA.DAS.Apprenticeships.TestHelpers;

public class DatabaseInfo
{
    public string ConnectionString { get; private set; }
    public string DatabaseName { get; private set; }

    public DatabaseInfo(string connectionString = null)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }
        SetConnectionString(connectionString);
    }

    public void SetConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public void SetDatabaseName(string databaseName)
    {
        DatabaseName = databaseName;
    }
}