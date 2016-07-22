using System.Runtime.Serialization;

namespace Microsoft.Azure.ApiHub.Table.Protocol
{
    /// <summary>
    /// Represents a dataset
    /// </summary>
    [DataContract]
    internal class DataSet
    {
        /// <summary>
        /// Gets or sets the dataset name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets dataset display name
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }
    }
}
