// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Protocol = Microsoft.Azure.ApiHub.Table.Protocol;

namespace Microsoft.Azure.ApiHub
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

        Contains = Protocol.CapabilityFilterFunction.Contains,

        StartsWith = Protocol.CapabilityFilterFunction.StartsWith,

        EndsWith = Protocol.CapabilityFilterFunction.EndsWith,

        Length = Protocol.CapabilityFilterFunction.Length,

        IndexOf = Protocol.CapabilityFilterFunction.IndexOf,

        Replace = Protocol.CapabilityFilterFunction.Replace,

        Substring = Protocol.CapabilityFilterFunction.Substring,

        SubstringOf = Protocol.CapabilityFilterFunction.SubstringOf,

        ToLower = Protocol.CapabilityFilterFunction.ToLower,

        ToUpper = Protocol.CapabilityFilterFunction.ToUpper,

        Trim = Protocol.CapabilityFilterFunction.Trim,

        Concat = Protocol.CapabilityFilterFunction.Concat,

        Year = Protocol.CapabilityFilterFunction.Year,

        Month = Protocol.CapabilityFilterFunction.Month,

        Day = Protocol.CapabilityFilterFunction.Day,

        Hour = Protocol.CapabilityFilterFunction.Hour,

        Minute = Protocol.CapabilityFilterFunction.Minute,

        Second = Protocol.CapabilityFilterFunction.Second,

        Floor = Protocol.CapabilityFilterFunction.Floor,

        Ceiling = Protocol.CapabilityFilterFunction.Ceiling,

        Round = Protocol.CapabilityFilterFunction.Round,

        Not = Protocol.CapabilityFilterFunction.Not,

        Negate = Protocol.CapabilityFilterFunction.Negate,
    }
}
