using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ExpenseShare
    {
        public int ExpenseShareId { get; set; }
        public int ExpenseId { get; set; }
        public Expense Expense { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal ShareAmount { get; set; }
        public bool IsPaid { get; set; }=false;
    }
}
