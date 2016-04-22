using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.ApiHub.Sdk.Common.Protocol;
using Microsoft.Azure.ApiHub.Sdk.Extensions;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.ApiHub.Sdk.Tabular.Internal
{
    internal class ProtocolToModelConverter
    {
        public virtual DataSet Convert(Protocol.DataSet dataSet, TableClient provider)
        {
            var result = provider.GetDataSetReference(dataSet.Name);

            result.DisplayName = dataSet.DisplayName;

            return result;
        }

        public virtual Table Convert(Protocol.Table table, DataSet dataSet)
        {
            var result = dataSet.GetTableReference(table.Name);

            result.DisplayName = table.DisplayName;

            return result;
        }

        public virtual TableMetadata Convert(Protocol.TableMetadata metadata)
        {
            return new TableMetadata
            {
                Name = metadata.Name,
                Title = metadata.Title,
                Permission = metadata.Permission,
                Capabilities = Convert(metadata.Capabilities.Coalesce()),
                Schema = (JObject) metadata.Schema.Coalesce().DeepClone()
            };
        }

        protected virtual TableCapabilitiesMetadata Convert(Protocol.TableCapabilitiesMetadata metadata)
        {
            return new TableCapabilitiesMetadata
            {
                SortRestrictions = Convert(metadata.SortRestrictions.Coalesce()),
                FilterRestrictions = Convert(metadata.FilterRestrictions.Coalesce()),
                FilterFunctions = metadata.FilterFunctions.Coalesce().Select(Convert).ToList()
            };
        }

        protected virtual TableFilterRestrictionsMetadata Convert(Protocol.TableFilterRestrictionsMetadata metadata)
        {
            return new TableFilterRestrictionsMetadata
            {
                Filterable = metadata.Filterable,
                NonFilterableProperties = metadata.NonFilterableProperties.Coalesce().ToList(),
                RequiredProperties = metadata.RequiredProperties.Coalesce().ToList()
            };
        }

        protected virtual TableSortRestrictionsMetadata Convert(Protocol.TableSortRestrictionsMetadata metadata)
        {
            return new TableSortRestrictionsMetadata
            {
                Sortable = metadata.Sortable,
                UnsortableProperties = metadata.UnsortableProperties.Coalesce().ToList(),
                AscendingOnlyProperties = metadata.AscendingOnlyProperties.Coalesce().ToList()
            };
        }

        protected virtual CapabilityFilterFunction Convert(Protocol.CapabilityFilterFunction filterFunction)
        {
            return (CapabilityFilterFunction)filterFunction;
        }
    }
}
