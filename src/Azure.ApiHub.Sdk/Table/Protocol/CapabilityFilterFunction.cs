// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Runtime.Serialization;

namespace Microsoft.Azure.ApiHub.Table.Protocol
{
    /// <summary>
    /// Filter functions that could be available on a table
    /// </summary>
    [DataContract]
    internal enum CapabilityFilterFunction
    {
        /// <summary>
        /// The equal filter predicate
        /// </summary>
        [EnumMember(Value = "eq")]
        Equal,

        /// <summary>
        /// The not equal filter predicate
        /// </summary>
        [EnumMember(Value = "ne")]
        NotEqual,

        /// <summary>
        /// The greater than filter predicate
        /// </summary>
        [EnumMember(Value = "gt")]
        GreaterThan,

        /// <summary>
        /// The greater than or equal filter predicate
        /// </summary>
        [EnumMember(Value = "ge")]
        GreaterThanOrEqual,

        /// <summary>
        /// The less than filter predicate
        /// </summary>
        [EnumMember(Value = "lt")]
        LessThan,

        /// <summary>
        /// The less than or equal filter predicate
        /// </summary>
        [EnumMember(Value = "le")]
        LessThanOrEqual,

        /// <summary>
        /// The and filter operand
        /// </summary>
        [EnumMember(Value = "and")]
        And,

        /// <summary>
        /// The or filter operand
        /// </summary>
        [EnumMember(Value = "or")]
        Or,

        /// <summary>
        /// The contains filter predicate
        /// </summary>
        [EnumMember(Value = "contains")]
        Contains,

        /// <summary>
        /// The starts with predicate
        /// </summary>
        [EnumMember(Value = "startswith")]
        StartsWith,

        /// <summary>
        /// The ends with predicate
        /// </summary>
        [EnumMember(Value = "endswith")]
        EndsWith,

        /// <summary>
        /// The length filter function
        /// </summary>
        [EnumMember(Value = "length")]
        Length,

        /// <summary>
        /// The index of filter predicate
        /// </summary>
        [EnumMember(Value = "indexof")]
        IndexOf,

        /// <summary>
        /// The replace filter function
        /// </summary>
        [EnumMember(Value = "replace")]
        Replace,

        /// <summary>
        /// The substring filter function (OData v4)
        /// </summary>
        [EnumMember(Value = "substring")]
        Substring,

        /// <summary>
        /// The substring of filter function (OData v2)
        /// </summary>
        [EnumMember(Value = "substringof")]
        SubstringOf,

        /// <summary>
        /// The to lower filter function
        /// </summary>
        [EnumMember(Value = "tolower")]
        ToLower,

        /// <summary>
        /// The to upper filter function
        /// </summary>
        [EnumMember(Value = "toupper")]
        ToUpper,

        /// <summary>
        /// The trim filter function
        /// </summary>
        [EnumMember(Value = "trim")]
        Trim,

        /// <summary>
        /// The concatenate filter function
        /// </summary>
        [EnumMember(Value = "concat")]
        Concat,

        /// <summary>
        /// The year filter function
        /// </summary>
        [EnumMember(Value = "year")]
        Year,

        /// <summary>
        /// The month filter function
        /// </summary>
        [EnumMember(Value = "month")]
        Month,

        /// <summary>
        /// The day filter function
        /// </summary>
        [EnumMember(Value = "day")]
        Day,

        /// <summary>
        /// The hour filter function
        /// </summary>
        [EnumMember(Value = "hour")]
        Hour,

        /// <summary>
        /// The minute filter function
        /// </summary>
        [EnumMember(Value = "minute")]
        Minute,

        /// <summary>
        /// The second filter function
        /// </summary>
        [EnumMember(Value = "second")]
        Second,

        /// <summary>
        /// The floor filter function
        /// </summary>
        [EnumMember(Value = "floor")]
        Floor,

        /// <summary>
        /// The ceiling filter function
        /// </summary>
        [EnumMember(Value = "ceiling")]
        Ceiling,

        /// <summary>
        /// The round filter function
        /// </summary>
        [EnumMember(Value = "round")]
        Round,

        /// <summary>
        /// The not unary filter function
        /// </summary>
        [EnumMember(Value = "not")]
        Not,

        /// <summary>
        /// The negate unary filter function
        /// </summary>
        [EnumMember(Value = "-")]
        Negate,
    }
}
