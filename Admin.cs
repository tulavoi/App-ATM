using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAtm
{
    internal class Admin
    {
        // Fields
        private string username, password;


        // Constructors
        public Admin()
        {
        }

        public Admin(string username, string password)
        {
            this.username = username;
            this.password = password;
        }


        // Properties
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
    }
}
