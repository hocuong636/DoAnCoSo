using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CT_Fas.Models
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}