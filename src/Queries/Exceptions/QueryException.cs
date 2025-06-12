namespace SFA.DAS.Learning.Queries.Exceptions
{
    [Serializable]
    public sealed class QueryException : Exception
    {
        public QueryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
