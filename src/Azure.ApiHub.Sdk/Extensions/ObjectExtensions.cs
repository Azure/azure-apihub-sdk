// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.ApiHub.Extensions
{
    internal static class ObjectExtensions
    {
        public static T Coalesce<T>(this T obj)
            where T: class, new()
        {
            return obj ?? new T();
        }
    }
}
