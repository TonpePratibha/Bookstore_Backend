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
        public void RegisterAdmin(AdminModel adminModel);
        public string ValidateAdmin(AdminLogin adminLoginModel);

        public AdminModel getAdinById(int id);
    }
}
