using DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;

        public ExpenseService(IExpenseRepository expenseRepository, IUserRepository userRepository, IGroupRepository groupRepository)
        {
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            var group = await _groupRepository.GetGroupByIdAsync(expense.GroupId);
            if (group == null) throw new Exception("Group not found");

            var groupMembers = group.Members.Count();
            if (groupMembers == 0) throw new Exception("No members in the group");

            // Calculate the integer amount each member will get
            var splitAmount = (int)(expense.Amount / groupMembers);
            // Calculate the remainder after integer division
            var remainder = expense.Amount % groupMembers;

            // List to track how much each member will receive initially
            var amounts = new List<int>();
            for (int i = 0; i < groupMembers; i++)
            {
                amounts.Add(splitAmount);
            }

            // Distribute the remainder by adding 1 to each member starting from the first
            for (int i = 0; i < remainder; i++)
            {
                amounts[i] += 1;
            }

            // Save the expense first to generate the ExpenseId
            await _expenseRepository.AddExpenseAsync(expense);
            await _expenseRepository.SaveChangesAsync(); // This will generate the ExpenseId
            var index = 0;
            foreach (var member in group.Members)
            {
                var user = await _userRepository.GetUserByIdAsync(member.UserId);
                if (user != null)
                {
                    user.AmountOwed += amounts[index];

                    // Create and add ExpenseShare
                    var expenseShare = new ExpenseShare
                    {
                        ExpenseId = expense.ExpenseId,
                        UserId = user.UserId,
                        ShareAmount = amounts[index]
                    };
                    await _expenseRepository.AddExpenseShareAsync(expenseShare);

                    // Use the index to assign the appropriate amount
                    if (member.UserId == expense.PaidById)
                    {
                        user.AmountOwedTo += expense.Amount;
                        user.AmountOwedTo -= user.AmountOwed;

                    }
                    await _userRepository.UpdateUserAsync(user);
                }
                index++;
            }

            //await _expenseRepository.AddExpenseAsync(expense);
            await _expenseRepository.SaveChangesAsync();

        }
        public async Task<Expense> GetExpenseByIdAsync(int expenseId)
        {
            return await _expenseRepository.GetExpenseByIdAsync(expenseId);
        }

        public async Task DeleteExpenseAsync(int expenseId)
        {

            var expense = await _expenseRepository.GetExpenseByIdAsync(expenseId);
            if (expense == null) return;

            var group = await _groupRepository.GetGroupByIdAsync(expense.GroupId);
            if (group == null) throw new Exception("Group not found");
            var groupMembers = group.Members.Count;

            // Calculate the integer amount each member will get
            var splitAmount = (int)(expense.Amount / groupMembers);
            // Calculate the remainder after integer division
            var remainder = expense.Amount % groupMembers;

            // List to track how much each member will receive initially
            var amounts = new List<int>();
            for (int i = 0; i < groupMembers; i++)
            {
                amounts.Add(splitAmount);
            }

            // Distribute the remainder by adding 1 to each member starting from the first
            for (int i = 0; i < remainder; i++)
            {
                amounts[i] += 1;
            }

            foreach (var member in group.Members)
            {
                var user = await _userRepository.GetUserByIdAsync(member.UserId);
                if (user != null)
                {
                    user.AmountOwed -= amounts[user.UserId - 1];
                    if (member.UserId == expense.PaidById)
                    {
                        user.AmountOwedTo -= expense.Amount - amounts[user.UserId - 1];
                    }
                    await _userRepository.UpdateUserAsync(user);
                }
            }

            await _expenseRepository.DeleteExpenseAsync(expense);
        }
        public async Task<bool> PayExpenseAsync(int expenseShareId, int userId)
        {
            // Fetch the expense share by ID
            var expenseShare = await _expenseRepository.GetExpenseShareByIdAsync(expenseShareId);

            if (expenseShare == null)
            {
                return false; // ExpenseShare not found
            }

            // Fetch the expense related to the expenseShare
            var expense = await _expenseRepository.GetExpenseByIdAsync(expenseShare.ExpenseId);

            if (expense == null)
            {
                return false; // Expense not found
            }

            // Fetch the user and the user who paid for the expense
            var user = await _userRepository.GetUserByIdAsync(userId);
            var paidByUser = await _userRepository.GetUserByIdAsync(expense.PaidById);

            if (user == null || paidByUser == null)
            {
                return false; // User or PaidByUser not found
            }

            // Clear the owed amount
            user.AmountOwed -= expenseShare.ShareAmount;

            // Add the amount to the PaidByUser's AmountOwedTo
            paidByUser.AmountOwedTo += expenseShare.ShareAmount;

            // Mark the expense share as paid
            expenseShare.IsPaid = true;
            await _expenseRepository.UpdateExpenseShareAsync(expenseShare);

            // Save changes
            await _expenseRepository.SaveChangesAsync();
            await _userRepository.SaveChangesAsync();

            return true;
        }
        public async Task<List<ExpenseShare>> GetUnpaidExpenseSharesForUserAsync(int userId)
        {
            return await _expenseRepository.GetUnpaidExpenseSharesForUserAsync(userId);
        }
        public async Task<ExpenseShare> GetExpenseShareByIdAsync(int expenseShareId)
        {
            return await _expenseRepository.GetExpenseShareByIdAsync(expenseShareId);
        }
    }  
}
