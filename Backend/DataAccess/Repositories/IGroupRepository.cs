using System;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IGroupRepository
    {
        Task<Group> GetGroupByIdAsync(int groupId);
        Task AddGroupAsync(Group group);
        Task UpdateGroupAsync(Group group);
        Task DeleteGroupAsync(Group group);
        Task<IEnumerable<Group>> GetAllGroupsAsync();
        Task<List<Group>> GetGroupsUserIsNotInAsync(int userId);
        Task<List<Expense>> GetExpensesByGroupIdAsync(int groupId);
        Task<List<ExpenseShare>> GetExpenseSharesByExpenseIdAsync(int expenseId);

    }
}
