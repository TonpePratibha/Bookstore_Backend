using DataAccessLayer.Entity;
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
        public UserModel RegisterUser(UserModel userModel);
        public LoginResponse ValidateUser(UserLogin userLoginModel);
        public UserModel GetUserById(int id);
        // public List<User> getAllUsers();
        public void DeleteUser(int id);
        List<User> GetAllUsers();
        public void UpdateUser(int id, UserModel model);
        public void SendResetPasswordEmail(string email);
        public string ResetPassword(string token, string newPassword);
        public RefreshLoginResponse AcesstokenLogin(UserLogin userLoginModel);
        public void SendEmail(string toEmail, string subject, string body);



        public RefreshLoginResponse RefreshAccessToken(string refreshToken);
    }
}
