using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IUserService
    {
        string SayHello(User user);
        IList<User> GetAllUsers();
    }
}
