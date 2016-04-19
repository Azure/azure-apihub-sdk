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
        /// Gets a reference to a file asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        Task<IFileItem> GetFileReferenceAsync(string path, bool overwrite = true);

        /// <summary>
        /// Gets a reference to a folder asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        Task<IFolderItem> GetFolderReferenceAsync(string path);

        /// <summary>
        /// Lists the files asynchronous.
        /// </summary>
        /// <param name="includeSubdirectories">if set to <c>true</c> [include subdirectories].</param>
        /// <returns></returns>
        Task<IItem[]> ListAsync(bool includeSubdirectories = false);

        /// <summary>
        /// Creates a file watcher.
        /// </summary>
        /// <param name="fileWatcherType">Type of the file watcher.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="nextItem">The next item to continue monitoring.</param>
        /// <param name="pollIntervalInSeconds">The poll interval in seconds.</param>
        /// <returns></returns>
        IFileWatcher CreateFileWatcher(FileWatcherType fileWatcherType, Func<IFileItem, object, Task> callback, object nextItem = null, int pollIntervalInSeconds = CdpConstants.DefaultFileWatcherIntervalInSeconds);

        /// <summary>
        /// Checks for a file to be added or updated.
        /// </summary>
        /// <param name="fileWatcherType">Type of the file watcher.</param>
        /// <param name="url">The next item to continue monitoring.</param>
        /// <returns></returns>
        Task<FileTriggerInfo> CheckForFile(FileWatcherType fileWatcherType, object nextItem = null);
    }
}
