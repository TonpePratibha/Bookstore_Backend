using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
   public interface IAdminRepository
    {
        public AdminModel RegisterAdmin(AdminModel adminModel);
        public LoginResponse ValidateAdmin(AdminLogin adminLoginModel);

        public AdminModel GetAdminById(int id);
        public void DeleteAdmin(int id);
        public void UpdateAdmin(int id, AdminModel model);
        public void SendResetPasswordEmail(string email);
        public string ResetPassword(string token, string newPassword);
        public RefreshLoginResponse AccesstokenLogin(AdminLogin adminLoginModel);
        public RefreshLoginResponse RefreshAccessToken(string refreshToken);
    }
}
