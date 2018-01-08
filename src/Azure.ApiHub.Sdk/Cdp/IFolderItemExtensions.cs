// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    public static class IFolderItemExtensions
    {
        public static IFolderItem GetFolderReference(this IFolderItem folder, string path, string handleId, Uri runtimeEndpoint, string scheme, string accessToken, ILogger logger = null)
        {
            return new FolderItem
            {
                _path = path,
                _handleId = handleId,
                _cdpHelper = new CdpHelper(runtimeEndpoint, scheme, accessToken, logger)
            };
        }

        public static async Task<bool> FileExistsAsync(this IFolderItem folder, string path)
        {
            if (await folder.GetFileItemAsync(path) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> FolderExistsAsync(this IFolderItem folder, string path)
        {
            if (await folder.GetFolderItemAsync(path) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
