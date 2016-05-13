using System.Runtime.Serialization;

namespace Microsoft.Azure.ApiHub.Sdk.Table.Protocol
{
    /// <summary>
    /// Filter functions that could be available on a table
    /// </summary>
    [DataContract]
    public enum CapabilityFilterFunction
    {
        /// <summary>
        /// The equal
        /// </summary>
        [EnumMember(Value = "eq")]
        Equal,

        /// <summary>
        /// The not equal
        /// </summary>
        [EnumMember(Value = "ne")]
        NotEqual,

        /// <summary>
        /// The greater than
        /// </summary>
        [EnumMember(Value = "gt")]
        GreaterThan,

        /// <summary>
        /// The greater than or equal
        /// </summary>
        [EnumMember(Value = "ge")]
        GreaterThanOrEqual,

        /// <summary>
        /// The less than
        /// </summary>
        [EnumMember(Value = "lt")]
        LessThan,

        /// <summary>
        /// The less than or equal
        /// </summary>
        [EnumMember(Value = "le")]
        LessThanOrEqual,

        /// <summary>
        /// The and
        /// </summary>
        [EnumMember(Value = "and")]
        And,

        /// <summary>
        /// The or
        /// </summary>
        [EnumMember(Value = "or")]
        Or,
    }
}
