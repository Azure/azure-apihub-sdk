namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    // TODO: Add QueryBuilder classes to allow strongly typed query creation.
    public class Query
    {
        internal string QueryString { get; set; }

        public static Query Parse(string queryString)
        {
            return new Query
            {
                QueryString = queryString
            };
        }
    }
}
