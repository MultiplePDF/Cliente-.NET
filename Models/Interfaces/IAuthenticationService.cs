using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace client.Models.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> Login(string user, string password);
        Task<bool> Logout(string token);
    }
}