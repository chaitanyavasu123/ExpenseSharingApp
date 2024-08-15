using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IExpenseRepository
    {
        Task<Expense> GetExpenseByIdAsync(int expenseId);
        Task AddExpenseAsync(Expense expense);
        Task UpdateExpenseAsync(Expense expense);
        Task DeleteExpenseAsync(Expense expense);
        Task<IEnumerable<Expense>> GetAllExpensesAsync();
        Task AddExpenseShareAsync(ExpenseShare expenseShare);
        Task<ExpenseShare> GetUserExpenseShareAsync(int expenseId, int userId);
        Task<List<ExpenseShare>> GetUnpaidExpenseSharesForUserAsync(int userId);
        Task<ExpenseShare> GetExpenseShareByIdAsync(int expenseShareId);
        Task UpdateExpenseShareAsync(ExpenseShare expenseShare);
        Task DeleteExpenseShareAsync(ExpenseShare expenseShare);
        Task SaveChangesAsync();
    }
}
