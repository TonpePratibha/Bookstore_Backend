using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
    public class WishListSummeryModel
    {
        public UserDetailsModel User { get; set; }
        public List<WishlListItemModel> Items { get; set; }
    }
}
