// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ApiHub.Table.Protocol;

namespace Microsoft.Azure.ApiHub.Table.Internal
{
    public class TableClient : ITableClient
    {
        public TableClient(ITabularConnectorAdapter adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            Adapter = adapter;
        }

        private ITabularConnectorAdapter Adapter { get; set; }

        public IDataSet GetDataSetReference(string dataSetName)
        {
            return new DataSet(dataSetName ?? Constants.DefaultDataSetName, Adapter);
        }

        public async Task<SegmentedResult<IDataSet>> ListDataSetsAsync(Query query, ContinuationToken continuationToken, CancellationToken cancellationToken)
        {
            return await Adapter.ListDataSetsAsync(
                query,
                continuationToken,
                cancellationToken);
        }
    }
}
