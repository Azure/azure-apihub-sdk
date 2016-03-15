using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    public interface IFileWatcher
    {
        Task StopAsync();
    }
}
