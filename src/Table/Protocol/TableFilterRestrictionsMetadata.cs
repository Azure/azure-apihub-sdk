using System.Runtime.Serialization;

namespace Microsoft.Azure.ApiHub.Table.Protocol
{
    /// <summary>
    /// Represents the metadata for a table.
    /// </summary>
    [DataContract]
    internal class TableFilterRestrictionsMetadata
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TableFilterRestrictionsMetadata"/> is filterable.
        /// </summary>
        [DataMember(Name = "filterable")]
        public bool Filterable { get; set; }

        /// <summary>
        /// Gets or sets the non filterable properties.
        /// </summary>
        [DataMember(Name = "nonFilterableProperties", EmitDefaultValue = false)]
        public string[] NonFilterableProperties { get; set; }

        /// <summary>
        /// Gets or sets the required properties.
        /// </summary>
        [DataMember(Name = "requiredProperties", EmitDefaultValue = false)]
        public string[] RequiredProperties { get; set; }
    }
}
