using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAtm
{
    internal class Card
    {
        // Fields
        private string id, pin;
        private int loginAttemptsCount;

        // Constructors
        public Card()
        {
        }


        public Card(string id, string pin, int loginAttemptsCount)
        {
            this.id = id;
            this.pin = pin;
            this.loginAttemptsCount = loginAttemptsCount;
        }


        // Properties
        public string Id { get => id; set => id = value; }
        public string Pin { get => pin; set => pin = value; }
        public int LoginAttemptsCount { get => loginAttemptsCount; set => loginAttemptsCount = value; }
    }
}
