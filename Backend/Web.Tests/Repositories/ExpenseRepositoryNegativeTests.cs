using DataAccess.Data;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.DataAccess
{
    public class ExpenseRepositoryNegativeTests
    {
        private readonly ExpenseSharingDbContext _context;
        private readonly ExpenseRepository _repository;

        public ExpenseRepositoryNegativeTests()
        {
            var options = new DbContextOptionsBuilder<ExpenseSharingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestExpenseDb_NegativeTests")
                .Options;

            _context = new ExpenseSharingDbContext(options);
            _repository = new ExpenseRepository(_context);
        }

        [Fact]
        public async Task GetExpenseByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var invalidExpenseId = 999;

            // Act
            var result = await _repository.GetExpenseByIdAsync(invalidExpenseId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddExpenseAsync_NullExpense_ThrowsException()
        {
            // Arrange
            Expense nullExpense = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddExpenseAsync(nullExpense));
        }

        [Fact]
        public async Task UpdateExpenseAsync_InvalidExpense_ThrowsException()
        {
            // Arrange
            var invalidExpense = new Expense { ExpenseId = 999, Description = "Invalid", Amount = 0 };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _repository.UpdateExpenseAsync(invalidExpense));
        }

       


        [Fact]
        public async Task GetAllExpensesAsync_NoExpenses_ReturnsEmptyList()
        {
            // Arrange
            // Ensure database is empty
            foreach (var expense in _context.Expenses.ToList())
            {
                _context.Expenses.Remove(expense);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllExpensesAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddExpenseShareAsync_NullExpenseShare_ThrowsException()
        {
            // Arrange
            ExpenseShare nullExpenseShare = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddExpenseShareAsync(nullExpenseShare));
        }

        [Fact]
        public async Task GetUserExpenseShareAsync_InvalidExpenseIdOrUserId_ReturnsNull()
        {
            // Arrange
            var invalidExpenseId = 999;
            var invalidUserId = 999;

            // Act
            var result = await _repository.GetUserExpenseShareAsync(invalidExpenseId, invalidUserId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUnpaidExpenseSharesForUserAsync_InvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            var invalidUserId = 999;

            // Act
            var result = await _repository.GetUnpaidExpenseSharesForUserAsync(invalidUserId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetExpenseShareByIdAsync_InvalidExpenseShareId_ReturnsNull()
        {
            // Arrange
            var invalidExpenseShareId = 999;

            // Act
            var result = await _repository.GetExpenseShareByIdAsync(invalidExpenseShareId);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task SaveChangesAsync_NoChanges_ThrowsNoException()
        {
            // Act & Assert
            await _repository.SaveChangesAsync();
        }
    }
}


