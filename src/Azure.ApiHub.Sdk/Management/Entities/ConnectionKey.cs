// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.ApiHub.Management.Entities
{
    public class ConnectionKey
    {
        public string Key { get; internal set; }

        public Uri RuntimeUri { get; internal set; }
    }
}
