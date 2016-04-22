namespace Microsoft.Azure.ApiHub.Sdk.Tabular
{
    public enum CapabilityFilterFunction
    {
        Equal = Protocol.CapabilityFilterFunction.Equal,

        NotEqual = Protocol.CapabilityFilterFunction.NotEqual,

        GreaterThan = Protocol.CapabilityFilterFunction.GreaterThan,

        GreaterThanOrEqual = Protocol.CapabilityFilterFunction.GreaterThanOrEqual,

        LessThan = Protocol.CapabilityFilterFunction.LessThan,

        LessThanOrEqual = Protocol.CapabilityFilterFunction.LessThanOrEqual,

        And = Protocol.CapabilityFilterFunction.And,

        Or = Protocol.CapabilityFilterFunction.Or,
    }
}
