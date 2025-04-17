using BuisnessLayer.Interface;
using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
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

    }
}
