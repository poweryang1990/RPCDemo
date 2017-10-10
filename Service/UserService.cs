using IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Service
{
    public class UserService : IUserService
    {
        public IList<User> GetAllUsers()
        {
            return new List<User>
            {
                new User{ name="AAAAAAAAAAA"},
                new User{name="BBBBB"}
            };
        }

        public string SayHello(User user)
        {
            return $"Hello {user.name} ";
        }
    }
}
