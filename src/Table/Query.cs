namespace Microsoft.Azure.ApiHub.Sdk.Table
{
    /// <summary>
    /// Defines a query to be executed against a tabular connector.
    /// </summary>
    // TODO: Add QueryBuilder classes to allow strongly typed query creation.
    public class Query
    {
        internal string QueryString { get; private set; }

        public static Query Parse(string queryString)
        {
            return new Query
            {
                QueryString = queryString
            };
        }
    }
}
