using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    public static class IFolderItemExtensions
    {
        public static Task<IFolderItem> GetFolderItemAsync(this IFolderItem folder, string path, string handleId, Uri runtimeEndpoint, string scheme, string accessToken)
        {            
            return Task.FromResult<IFolderItem>(
                new FolderItem
                {
                    _path = path,
                    _handleId = handleId,
                    _cdpHelper = new CdpHelper(runtimeEndpoint, scheme, accessToken)
                });
        }

        public static bool FileExists(this IFolderItem folder, string path)
        {
            if ( folder.GetFileItemAsync(path).GetAwaiter().GetResult() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool FolderExists(this IFolderItem folder, string path)
        {
            if (folder.GetFolderItemAsync(path).GetAwaiter().GetResult() != null)
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
