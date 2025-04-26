using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
 public class WishListModel
    {

        public int AddedBy { get; set; }  // UserId
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }

        public int BookId { get; set; }  
        public string BookName { get; set; }

    }
}
