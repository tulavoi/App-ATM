using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAtm
{
    internal class TransactionHistory
    {
        // Fields
        private string id, transactionType;
        private long amountMoney;
        private DateTime transactionTime;


        // Constructors
        public TransactionHistory()
        {
        }

        public TransactionHistory(string id, string transactionType, long amountMoney, DateTime transactionTime)
        {
            this.id = id;
            this.transactionType = transactionType;
            this.amountMoney = amountMoney;
            this.transactionTime = transactionTime;
        }


        // Properties
        public string Id { get => id; set => id = value; }
        public string TransactionType { get => transactionType; set => transactionType = value; }
        public long AmountMoney { get => amountMoney; set => amountMoney = value; }
        public DateTime TransactionTime { get => transactionTime; set => transactionTime = value; }
    }
}
