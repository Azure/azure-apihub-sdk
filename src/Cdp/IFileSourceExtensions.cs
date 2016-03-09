using System.Text;
using System.Threading.Tasks;

namespace Microfoft.Azure.ApiHub.Sdk.Cdp
{

    public static class IFileSourceExtensions
    {
        // Search for a path. 
        // This is a grossly inefficient alternative to datasets/default/GetFileByPath
        //public static async Task<DskItemInfo> SearchAsync(this IFileSource dsk, string path)
        //{
        //    string[] parts = path.Split('/');
        //    int i = 0;

        //    return await SearchHelperAsync(dsk, parts, i, null);
        //}

        //private static async Task<DskItemInfo> SearchHelperAsync(this IFileSource dsk, string[] parts, int idx, DskItemInfo current)
        //{
        //    if (idx >= parts.Length)
        //    {
        //        return current;
        //    }

        //    var items = await dsk.ListFolderAsync(current);

        //    string next = parts[idx];

        //    foreach (var item in items)
        //    {
        //        if (string.Equals(item.Name, next, StringComparison.OrdinalIgnoreCase))
        //        {
        //            if (!item.IsFolder)
        //            {
        //                return item;
        //            }
        //            return await SearchHelperAsync(dsk, parts, idx + 1, item);
        //        }
        //    }

        //    // Print where we lost the trail
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < idx; i++)
        //    {
        //        sb.Append("/");
        //        sb.Append(parts[i]);
        //    }
        //    string msg = string.Format("Not found: Trail lost at {0}", sb);
        //    throw new NotImplementedException(msg);
        //}

        //public static async Task<string> GetTextAsync(this IFileSource dsk, string path)
        //{
        //    var id = await dsk.GetFileIdAsync(path);
        //    return await GetTextAsync(dsk, id);
        //}

        //public static async Task<string> GetTextAsync(this IFileSource dsk, CdpItemInfo file)
        //{
        //    var bytes = await dsk.ReadAsync(file);
        //    return Encoding.UTF8.GetString(bytes);
        //}
    }
}
