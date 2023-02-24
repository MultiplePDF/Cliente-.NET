using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace client.Models.Interfaces
{
    public interface IRegistrationService 
    {
        Task<bool> SignUp(string name, string email, string password);
    }
}