using System.Runtime.Serialization;

namespace Microsoft.Azure.ApiHub.Table.Protocol
{
    /// <summary>
    /// Represents the metadata for a table.
    /// </summary>
    [DataContract]
    internal class TableCapabilitiesMetadata
    {
        /// <summary>
        /// Gets or sets the sort restrictions.
        /// </summary>
        [DataMember(Name = "sortRestrictions", EmitDefaultValue = false)]
        public TableSortRestrictionsMetadata SortRestrictions { get; set; }

        /// <summary>
        /// Gets or sets the filter restrictions.
        /// </summary>
        [DataMember(Name = "filterRestrictions", EmitDefaultValue = false)]
        public TableFilterRestrictionsMetadata FilterRestrictions { get; set; }

        /// <summary>
        /// Gets or sets the filter functions.
        /// </summary>
        [DataMember(Name = "filterFunctions", EmitDefaultValue = false)]
        public CapabilityFilterFunction[] FilterFunctions { get; set; }
    }
}
