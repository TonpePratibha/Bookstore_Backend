using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Interface
{
   public interface IUserService
    {
        public void RegisterUser(UserModel userModel);
        public string ValidateUser(UserLogin userLoginModel);
        
    }
}
