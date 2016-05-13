using System.Runtime.Serialization;

namespace Microsoft.Azure.ApiHub.Sdk.Table.Protocol
{
    /// <summary>
    /// Represents the metadata for a table.
    /// </summary>
    [DataContract]
    public class TableSortRestrictionsMetadata
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TableSortRestrictionsMetadata"/> is sortable.
        /// </summary>
        [DataMember(Name = "sortable")]
        public bool Sortable { get; set; }

        /// <summary>
        /// Gets or sets the unsortable properties.
        /// </summary>
        [DataMember(Name = "unsortableProperties", EmitDefaultValue = false)]
        public string[] UnsortableProperties { get; set; }

        /// <summary>
        /// Gets or sets the ascending only properties.
        /// </summary>
        [DataMember(Name = "ascendingOnlyProperties", EmitDefaultValue = false)]
        public string[] AscendingOnlyProperties { get; set; }
    }
}
