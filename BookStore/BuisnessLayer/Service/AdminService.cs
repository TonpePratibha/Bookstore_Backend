using BuisnessLayer.Interface;
using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
using DataAccessLayer.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Service
{
    public class AdminService : IAdminService
    {


        private readonly IAdminRepository _adminRepository;


        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;




        }
        public void RegisterAdmin(AdminModel adminModel) {

            _adminRepository.RegisterAdmin(adminModel);
        }


        public string ValidateAdmin(AdminLogin adminLoginModel) {

            var token = _adminRepository.ValidateAdmin(adminLoginModel);
            if (token == null)
            {
                return "token is null";
            }
            return token;
        }
        public AdminModel getAdminById(int id) { 
         return _adminRepository.getAdinById(id);

        }

        public void DeleteAdmin(int id) { 
         _adminRepository.DeleteAdmin(id);

        }

        public void UpdateAdmin(int id, AdminModel model)
        {
            _adminRepository.UpdateAdmin(id, model);
        }

        public void SendResetPasswordEmail(string email)
        {
            _adminRepository.SendResetPasswordEmail(email);
        }
        public string ResetPassword(string token, string newPassword)
        {
            return _adminRepository.ResetPassword(token, newPassword);
        }

    }
}
