using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microfoft.WindowsAzure.ApiHub
{
    // could be file or folder.
    public interface IItem
    {
        /// <summary>
        /// Gets the path. The returned path will have forward only slashes.
        /// The path does not end with a forward slash.
        /// Here are some examples: /cdpfiles/test.txt, /cdpfiles/nestedfolder, /cdpfiles/nestedfolder/test.json
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        string Path { get; }

        /// <summary>
        /// Gets the handle identifier.
        /// </summary>
        /// <value>
        /// The handle identifier.
        /// </value>
        Task<string> HandleId { get; }

        /// <summary>
        /// Gets the metadata asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<MetadataInfo> GetMetadataAsync();
    }
}
