
namespace client.Models.Interfaces
{
    public interface IFileManageService
    {
        Task<bool> LoadWebPageFiles(string token, List<string> filesWeb);
        Task<List<string>> SerializeWebPageFiles(string token, List<string> filesWeb);
        Task<bool> SendInformationToServer(string token, List<string> encodedInformation);
    }
}