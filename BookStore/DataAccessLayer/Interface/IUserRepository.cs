using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
 public interface IUserRepository
    {
        public void RegisterUser(UserModel userModel);
        public string ValidateUser(UserLogin userLoginModel);
    }
}
