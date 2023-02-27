using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace client.Models.Interfaces
{
    public interface IUserManagementService
    {
        Task<bool> ChangePassword(string currentPassword, string newPassword, string confirmNewPassword);
    }
}