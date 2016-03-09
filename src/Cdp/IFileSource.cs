using System;
using System.Threading.Tasks;

namespace Microfoft.Azure.ApiHub.Sdk.Cdp
{
    /// <summary>
    /// The IFileSource interface
    /// </summary>
    public interface IFileSource
    {
        /// <summary>
        /// Gets the file identifier asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<CdpItemInfo> GetIdAsync(string path);

        /// <summary>
        /// Creates the file asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        Task<CdpItemInfo> CreateAsync(string path, byte[] contents, bool overwrite = true);

        /// <summary>
        /// Reads the file asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<byte[]> ReadAsync(string path);

        /// <summary>
        /// Reads the file asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<byte[]> ReadAsync(CdpItemInfoBase id);

        /// <summary>
        /// Updates the file asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        Task<CdpItemInfo> UpdateAsync(CdpItemInfoBase id, byte[] contents);

        /// <summary>
        /// Deletes the file asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task DeleteAsync(CdpItemInfoBase id);

        /// <summary>
        /// Gets the file metadata asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<CdpItemInfo> GetMetaDataAsync(string path);

        /// <summary>
        /// Gets the file meta data asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<CdpItemInfo> GetMetaDataAsync(CdpItemInfoBase id);

        /// <summary>
        /// Lists the files asynchronous.
        /// </summary>
        /// <param name="id">The identifier or null for the root folder</param>
        /// <param name="includeSubdirectories">if set to <c>true</c> [include subdirectories].</param>
        /// <returns></returns>
        Task<CdpItemInfo[]> ListAsync(CdpItemInfoBase id = null, bool includeSubdirectories = false);

        /// <summary>
        /// Creates the new file watcher.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="pollIntervalInSeconds">The poll interval in seconds.</param>
        /// <returns></returns>
        IFileWatcher CreateNewFileWatcher(string folder, Func<CdpItemInfo, Task> callback, int pollIntervalInSeconds = CdpConstants.DefaultFileWatcherIntervalInSeconds);

        /// <summary>
        /// Creates the update file watcher.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="pollIntervalInSeconds">The poll interval in seconds.</param>
        /// <returns></returns>
        IFileWatcher CreateUpdateFileWatcher(string folder, Func<CdpItemInfo, Task> callback, int pollIntervalInSeconds = CdpConstants.DefaultFileWatcherIntervalInSeconds);
    }
}