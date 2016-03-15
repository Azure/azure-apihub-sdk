using System.Threading.Tasks;

namespace Microfoft.WindowsAzure.ApiHub
{
    public interface IFileWatcher
    {
        Task StopAsync();
    }
}
