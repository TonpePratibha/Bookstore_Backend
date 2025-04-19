using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
   public class RolebasedRefreshToken
    {

       
            public int Id { get; set; }

            public int EntityId { get; set; } // ID of the user/admin/restaurant
            public string Role { get; set; }  // "user", "admin", "restaurant"

            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }

            public DateTime AccessTokenExpiry { get; set; }
            public DateTime RefreshTokenExpiry { get; set; }
        }

 
}
