using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomException
{
    public class PageNotFoundException:Exception
    {
        public PageNotFoundException(string message):base(message) { }
    }
}
