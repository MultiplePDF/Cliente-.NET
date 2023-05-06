
namespace client.Models.Interfaces
{
    public interface IEditUserService
    {
        Task<string> EditUser(string name);
    }
}