using DataAccess.Repositories;
using Models;

namespace BusinessLogic.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IExpenseRepository _expenseRepository;

        public GroupService(IGroupRepository groupRepository, IUserRepository userRepository, IExpenseRepository expenseRepository)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _expenseRepository = expenseRepository;
        }

        public async Task CreateGroupAsync(Group group)
        {
            await _groupRepository.AddGroupAsync(group);
        }

        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            return await _groupRepository.GetGroupByIdAsync(groupId);
        }

        public async Task<bool> DeleteGroupAsync(int groupId, int currentUserId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group != null && group.CreatedById == currentUserId)
            {
                var expenses = await _groupRepository.GetExpensesByGroupIdAsync(groupId);
                foreach (var expense in expenses)
                {
                    var expenseShares = await _groupRepository.GetExpenseSharesByExpenseIdAsync(expense.ExpenseId);
                    foreach (var expenseShare in expenseShares)
                    {
                        await _expenseRepository.DeleteExpenseShareAsync(expenseShare);
                    }
                    await _expenseRepository.DeleteExpenseAsync(expense);
                }

                await _groupRepository.DeleteGroupAsync(group);
                return true; // Group deleted successfully
            }
            return false; // Group not found or current user is not the group creator
        }

        public async Task<bool> InviteUserToGroupAsync(int groupId, int userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null || group.Members.Count >= 10)
                return false;

            if (group.Members.Any(m => m.UserId == userId))
                return false;

            group.Members.Add(new GroupMember { GroupId = groupId, UserId = userId });
            await _groupRepository.UpdateGroupAsync(group);
            return true;
        }

        public async Task ExitGroupAsync(int groupId, int userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                return;

            var member = group.Members.SingleOrDefault(m => m.UserId == userId);
            if (member != null)
            {
                group.Members.Remove(member);
                await _groupRepository.UpdateGroupAsync(group);
            }
        }
        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await _groupRepository.GetAllGroupsAsync();
        }
        public async Task<List<Group>> GetGroupsUserIsNotInAsync(int userId)
        {
            return await _groupRepository.GetGroupsUserIsNotInAsync(userId);
        }
    }
}
