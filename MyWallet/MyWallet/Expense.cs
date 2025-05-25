using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWallet
{
    public class Expense
    {
        public string username { get; set; }
        public Category category { get; set; }
        public double amount { get; set; }
        public DateTime timestamp { get; set; }
        public Expense() { }
        public Expense(string username, Category category, double amount)
        {
            this.username = username;
            this.category = category;
            this.amount = amount;
            this.timestamp = DateTime.Now;
        }

        public Expense(string username, Category category, double amount, DateTime timestamp)
        {
            this.username = username;
            this.category = category;
            this.amount = amount;
            this.timestamp = timestamp;
        }
    }

}
