using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpenseSharingDbContext _context;

        public ExpenseRepository(ExpenseSharingDbContext context)
        {
            _context = context;
        }

        public async Task<Expense> GetExpenseByIdAsync(int expenseId)
        {
            return await _context.Expenses.FindAsync(expenseId);
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(Expense expense)
        {
            _context.Expenses.Remove(expense);
           
        }

        public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
        {
            return await _context.Expenses.ToListAsync();
        }
        public async Task AddExpenseShareAsync(ExpenseShare expenseShare)
        {
            await _context.ExpenseShares.AddAsync(expenseShare);
        }
        public async Task<ExpenseShare> GetUserExpenseShareAsync(int expenseId, int userId)
        {
            return await _context.ExpenseShares
                .FirstOrDefaultAsync(es => es.ExpenseId == expenseId && es.UserId == userId);
        }
        public async Task<List<ExpenseShare>> GetUnpaidExpenseSharesForUserAsync(int userId)
        {
            return await _context.ExpenseShares
                .Where(es => es.UserId == userId && !es.IsPaid)
                .Include(es => es.Expense) // Include related Expense entity
                .ToListAsync();
        }
        public async Task<ExpenseShare> GetExpenseShareByIdAsync(int expenseShareId)
        {
            return await _context.ExpenseShares.FindAsync(expenseShareId);
        }
        public async Task UpdateExpenseShareAsync(ExpenseShare expenseShare)
        {
            _context.ExpenseShares.Update(expenseShare);
        }
        public async Task DeleteExpenseShareAsync(ExpenseShare expenseShare)
        {
            _context.ExpenseShares.Remove(expenseShare);
            
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
