
namespace client.Models.Interfaces
{
    public interface IDownloadService
    {
        Task<string> ProvideDownloadLink(string token);
    }
}