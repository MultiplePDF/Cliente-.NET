
namespace client.Models.Interfaces
{
    public interface IPasswordService
    {
        Task<string> ForgotPassword(string email);

    }
}