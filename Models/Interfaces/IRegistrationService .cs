
namespace client.Models.Interfaces
{
    public interface IRegistrationService
    {
        Task<bool> SignUp(string name, string email, string password);
    }
}