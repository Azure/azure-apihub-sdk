using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    // File may not yet exist. 
    public interface IFileItem : IItem
    {
        /// <summary>
        /// Reads the file asynchronous.
        /// </summary>
        /// <returns>If the file doesn't exist, it throws a FileNotFound exception.</returns>
        Task<byte[]> ReadAsync();

        /// <summary>
        /// Updates the file asynchronous.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        Task WriteAsync(byte[] contents);

        /// <summary>
        /// Deletes the file asynchronous.
        /// </summary>
        /// <returns>If the file doesn't exist then it does nothing.</returns>
        Task DeleteAsync();
    }
}
