using DataAccessLayer.Modal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Interface
{
public interface IAdminService
    {
        public void RegisterAdmin(AdminModel adminModel);
        public string ValidateAdmin(AdminLogin adminLoginModel);

        public AdminModel getAdminById(int id);
        public void DeleteAdmin(int id);
        public void SendResetPasswordEmail(string email);
        public string ResetPassword(string token, string newPassword);
        public void UpdateAdmin(int id, AdminModel model);



    }
}
