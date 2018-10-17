using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Taxi.Services
{
    public class EmailSenderOptions
    {
        public bool EnableSSL { get; set; }
        public string EmailFromName { get; set; }
        public string EmailFromAddress { get; set; }
        public string Username { get;  set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
