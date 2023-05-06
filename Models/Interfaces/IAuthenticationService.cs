using ServiceReference1;

namespace client.Models.Interfaces
{
    public interface IAuthenticationService
    {
        Task<loginResponse> Login(LoginModel model);
    }
}