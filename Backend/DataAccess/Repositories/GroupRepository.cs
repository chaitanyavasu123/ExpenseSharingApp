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
    public class GroupRepository : IGroupRepository
    {
        private readonly ExpenseSharingDbContext _context;

        public GroupRepository(ExpenseSharingDbContext context)
        {
            _context = context;
        }

        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            return await _context.Groups
        .Include(g => g.Members)
        .ThenInclude(m => m.User)
        .Include(g => g.Expenses)
        .FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task AddGroupAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGroupAsync(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(Group group)
        {
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups.ToListAsync();
        }
        public async Task<List<Group>> GetGroupsUserIsNotInAsync(int userId)
        {
            return await _context.Groups
                .Where(g => !g.Members.Any(gm => gm.UserId == userId))
                .ToListAsync();
        }
        public async Task<List<Expense>> GetExpensesByGroupIdAsync(int groupId)
        {
            return await _context.Expenses.Where(e => e.GroupId == groupId).ToListAsync();
        }

        public async Task<List<ExpenseShare>> GetExpenseSharesByExpenseIdAsync(int expenseId)
        {
            return await _context.ExpenseShares.Where(es => es.ExpenseId == expenseId).ToListAsync();
        }
    }
}
