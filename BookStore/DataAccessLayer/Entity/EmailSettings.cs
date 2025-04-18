using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public class EmailSettings
    {

        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string FromEmail { get; set; }
        public string Password { get; set; }
    }
}
