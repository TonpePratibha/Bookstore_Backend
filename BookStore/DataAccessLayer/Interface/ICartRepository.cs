using DataAccessLayer.Entity;
using DataAccessLayer.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
   public interface ICartRepository
    {
        public string AddToCart(string token, int bookId);
    }
}
