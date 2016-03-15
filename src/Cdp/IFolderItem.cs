using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    // Can represent $root, or a file. 
    // In all cases, the 'path' is relative to this folder. Uses / separator.
    public interface IFolderItem : IItem
    {
        /// <summary>
        /// Gets the file identifier asynchronous.
        /// </summary>
        /// <param name="path">The full path, relative to this folder.</param>
        /// <returns></returns>
        Task<IFileItem> GetFileItemAsync(string path);

        /// <summary>
        /// Gets the folder item asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<IFolderItem> GetFolderItemAsync(string path);

        /// <summary>
        /// Creates the file asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        Task<IFileItem> CreateFileAsync(string path, bool overwrite = true);

        /// <summary>
        /// Creates the folder asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        Task<IFolderItem> CreateFolderAsync(string path);

        /// <summary>
        /// Lists the files asynchronous.
        /// </summary>
        /// <param name="includeSubdirectories">if set to <c>true</c> [include subdirectories].</param>
        /// <returns></returns>
        Task<IItem[]> ListAsync(bool includeSubdirectories = false);

        /// <summary>
        /// Creates the new file watcher.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="pollIntervalInSeconds">The poll interval in seconds.</param>
        /// <returns></returns>
        IFileWatcher CreateNewFileWatcher(Func<IFileItem, Task> callback, int pollIntervalInSeconds = CdpConstants.DefaultFileWatcherIntervalInSeconds);

        /// <summary>
        /// Creates the update file watcher.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="pollIntervalInSeconds">The poll interval in seconds.</param>
        /// <returns></returns>
        IFileWatcher CreateUpdateFileWatcher(Func<IFileItem, Task> callback, int pollIntervalInSeconds = CdpConstants.DefaultFileWatcherIntervalInSeconds);
    }
}
