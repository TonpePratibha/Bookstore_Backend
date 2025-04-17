using DataAccessLayer.Modal;
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
    }
}
