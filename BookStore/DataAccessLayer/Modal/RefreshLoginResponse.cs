using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
    public class RefreshLoginResponse
    {
        
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string Email { get; set; }
        
            public string FirstName { get; set; }
        }

    


}

