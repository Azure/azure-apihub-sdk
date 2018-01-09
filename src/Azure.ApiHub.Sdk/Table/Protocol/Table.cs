// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Runtime.Serialization;

namespace Microsoft.Azure.ApiHub.Table.Protocol
{
    /// <summary>
    /// Represents a table
    /// </summary>
    [DataContract]
    internal class Table
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
