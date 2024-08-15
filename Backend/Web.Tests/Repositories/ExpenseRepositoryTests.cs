using DataAccess.Data;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.DataAccess
{
    public class ExpenseRepositoryTests
    {
        private readonly ExpenseSharingDbContext _context;
        private readonly ExpenseRepository _repository;

        public ExpenseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ExpenseSharingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestExpenseDb")
                .Options;

            _context = new ExpenseSharingDbContext(options);
            _repository = new ExpenseRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Expenses.Any())
            {
                var expenses = new List<Expense>
                {
                    new Expense { ExpenseId = 1, Description = "Lunch", Amount = 50, GroupId = 1, PaidById = 1 },
                    new Expense { ExpenseId = 2, Description = "Dinner", Amount = 100, GroupId = 2, PaidById = 2 }
                };

                var expenseShares = new List<ExpenseShare>
                {
                    new ExpenseShare { ExpenseShareId = 1, ExpenseId = 1, UserId = 1, IsPaid = false },
                    new ExpenseShare { ExpenseShareId = 2, ExpenseId = 1, UserId = 2, IsPaid = true },
                    new ExpenseShare { ExpenseShareId = 3, ExpenseId = 2, UserId = 1, IsPaid = false }
                };

                _context.Expenses.AddRange(expenses);
                _context.ExpenseShares.AddRange(expenseShares);
                _context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetExpenseByIdAsync_ValidId_ReturnsExpense()
        {
            // Arrange
            var expenseId = 1;

            // Act
            var result = await _repository.GetExpenseByIdAsync(expenseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expenseId, result.ExpenseId);
        }

        [Fact]
        public async Task AddExpenseAsync_ValidExpense_ExpenseAdded()
        {
            // Arrange
            var expense = new Expense { ExpenseId = 3, Description = "Breakfast", Amount = 30, GroupId = 1, PaidById = 1 };

            // Act
            await _repository.AddExpenseAsync(expense);
            var result = await _context.Expenses.FindAsync(expense.ExpenseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expense.Description, result.Description);
        }

        [Fact]
        public async Task UpdateExpenseAsync_ValidExpense_ExpenseUpdated()
        {
            // Arrange
            var expense = await _context.Expenses.FindAsync(1);
            expense.Description = "Updated Lunch";

            // Act
            await _repository.UpdateExpenseAsync(expense);
            var result = await _context.Expenses.FindAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Lunch", result.Description);
        }

        

        [Fact]
        public async Task GetAllExpensesAsync_ReturnsAllExpenses()
        {
            // Act
            var result = await _repository.GetAllExpensesAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }
        [Fact]
        public async Task AddExpenseShareAsync_ValidExpenseShare_AddsExpenseShare()
        {
            // Arrange
            var expenseShare = new ExpenseShare { ExpenseShareId = 4, ExpenseId = 1, UserId = 3, IsPaid = false };

            // Act
            await _repository.AddExpenseShareAsync(expenseShare);
            var result = await _context.ExpenseShares.FindAsync(4);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expenseShare.UserId, result.UserId);
        }

        [Fact]
        public async Task GetUserExpenseShareAsync_ValidExpenseIdAndUserId_ReturnsExpenseShare()
        {
            // Arrange
            var expenseId = 1;
            var userId = 1;

            // Act
            var result = await _repository.GetUserExpenseShareAsync(expenseId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expenseId, result.ExpenseId);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetUserExpenseShareAsync_InvalidExpenseIdOrUserId_ReturnsNull()
        {
            // Arrange
            var expenseId = 99;
            var userId = 99;

            // Act
            var result = await _repository.GetUserExpenseShareAsync(expenseId, userId);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task GetExpenseShareByIdAsync_ValidExpenseShareId_ReturnsExpenseShare()
        {
            // Arrange
            var expenseShareId = 2;

            // Act
            var result = await _repository.GetExpenseShareByIdAsync(expenseShareId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expenseShareId, result.ExpenseShareId);
        }

        [Fact]
        public async Task GetExpenseShareByIdAsync_InvalidExpenseShareId_ReturnsNull()
        {
            // Arrange
            var expenseShareId = 99;

            // Act
            var result = await _repository.GetExpenseShareByIdAsync(expenseShareId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateExpenseShareAsync_ValidExpenseShare_UpdatesExpenseShare()
        {
            // Arrange
            var expenseShare = await _context.ExpenseShares.FindAsync(1);
            expenseShare.IsPaid = true;

            // Act
            await _repository.UpdateExpenseShareAsync(expenseShare);
            var result = await _context.ExpenseShares.FindAsync(1);

            // Assert
            Assert.True(result.IsPaid);
        }
    }
}

