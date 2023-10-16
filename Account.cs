using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAtm
{
    internal class Account
    {
        // Fields
        private string id, name, typeCurrency;
        private long balance;


        // Constructors
        public Account()
        {
        }

        public Account(string id, string name, string typeCurrency, long balance)
        {
            this.id = id;
            this.name = name;
            this.typeCurrency = typeCurrency;
            this.balance = balance;
        }


        // Properties
        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string TypeCurrency { get => typeCurrency; set => typeCurrency = value; }
        public long Balance { get => balance; set => balance = value; }
    }
}
