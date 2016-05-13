using System.Runtime.Serialization;

namespace Microsoft.Azure.ApiHub.Sdk.Table.Protocol
{
    /// <summary>
    /// Represents a dataset
    /// </summary>
    [DataContract]
    public class DataSet
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
