using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IExpenseService
    {
        Task AddExpenseAsync(Expense expense);
        Task<Expense> GetExpenseByIdAsync(int expenseId);
        Task DeleteExpenseAsync(int expenseId);
        Task<bool> PayExpenseAsync(int expenseId, int userId);
        Task<List<ExpenseShare>> GetUnpaidExpenseSharesForUserAsync(int userId);
        Task<ExpenseShare> GetExpenseShareByIdAsync(int expenseShareId);

    }
}
