using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int PaidById { get; set; }
        public User? PaidBy { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public ICollection<ExpenseShare> ExpenseShares { get; set; }
    }
}
