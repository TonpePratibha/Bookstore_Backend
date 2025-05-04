using DataAccessLayer.DataContext;
using DataAccessLayer.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Modal
{
    public class OrderResponse
    {

        public string Message { get; set; }
        public List<OrderResponseModel> Orders { get; set; }
    }
}
