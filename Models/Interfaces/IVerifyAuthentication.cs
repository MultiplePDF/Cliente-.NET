
namespace client.Models.Interfaces
{
    public interface IVerifyAuthentication
    {
        Task<bool> VerifyAuthentication(string token);
    }
}