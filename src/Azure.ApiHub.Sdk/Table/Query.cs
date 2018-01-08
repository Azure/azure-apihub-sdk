// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.ApiHub
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
