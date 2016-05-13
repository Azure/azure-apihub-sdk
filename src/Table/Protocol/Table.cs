using System.Runtime.Serialization;

namespace Microsoft.Azure.ApiHub.Sdk.Table.Protocol
{
    /// <summary>
    /// Represents a table
    /// </summary>
    [DataContract]
    public class Table
    {
        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets table display name
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }
    }
}
