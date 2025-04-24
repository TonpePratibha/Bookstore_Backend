using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
    public class CartModel
    {
     
            public int PurchasedBy { get; set; }  // UserId

            public int BookId { get; set; }

            public int Quantity { get; set; }

            public decimal Price { get; set; }

           // public bool IsPurchased { get; set; }=false;
        }
    }

