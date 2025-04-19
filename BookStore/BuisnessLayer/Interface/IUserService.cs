using DataAccessLayer.Entity;
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
        public UserModel RegisterUser(UserModel userModel);
        public LoginResponse ValidateUser(UserLogin userLoginModel);

        public UserModel getUserById(int id);
       
        public void Deleteuser(int id);
        public List<UserModel> GetAllUsers();
        public void UpdateUser(int id, UserModel model);
        public void SendResetPasswordEmail(string email);
        public string ResetPassword(string token, string newPassword);
        public RefreshLoginResponse AcesstokenLogin(UserLogin userLoginModel);
        public RefreshLoginResponse RefreshAccessToken(string refreshToken);




        }
}
