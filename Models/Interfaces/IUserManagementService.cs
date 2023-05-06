
namespace client.Models.Interfaces
{
    public interface IUserManagementService
    {
        Task<bool> ChangePassword(string currentPassword, string newPassword, string confirmNewPassword);
    }
}