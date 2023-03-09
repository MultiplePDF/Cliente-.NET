using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace client.Models.Interfaces
{
    public interface IEditUserService
    {
        Task<string> EditUser(string name);
    }
}