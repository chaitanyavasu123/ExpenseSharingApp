using BusinessLogic.Services;
using DataAccess.Repositories;
using Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BusinessLogic
{
    public class ExpenseServiceNegativeTests
    {
        private readonly Mock<IExpenseRepository> _mockExpenseRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly ExpenseService _expenseService;

        public ExpenseServiceNegativeTests()
        {
            _mockExpenseRepository = new Mock<IExpenseRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _expenseService = new ExpenseService(
                _mockExpenseRepository.Object,
                _mockUserRepository.Object,
                _mockGroupRepository.Object);
        }

        [Fact]
        public async Task AddExpenseAsync_GroupNotFound_ThrowsException()
        {
            // Arrange
            var expense = new Expense { GroupId = 1, Amount = 100, PaidById = 1 };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _expenseService.AddExpenseAsync(expense));
            Assert.Equal("Group not found", exception.Message);
        }

        [Fact]
        public async Task AddExpenseAsync_NoMembersInGroup_ThrowsException()
        {
            // Arrange
            var expense = new Expense { GroupId = 1, Amount = 100, PaidById = 1 };
            var group = new Group { GroupId = 1, Members = new List<GroupMember>() };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(group);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _expenseService.AddExpenseAsync(expense));
            Assert.Equal("No members in the group", exception.Message);
        }

        [Fact]
        public async Task GetExpenseByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            _mockExpenseRepository.Setup(repo => repo.GetExpenseByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Expense)null);

            // Act
            var result = await _expenseService.GetExpenseByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteExpenseAsync_ExpenseNotFound_DoesNothing()
        {
            // Arrange
            _mockExpenseRepository.Setup(repo => repo.GetExpenseByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Expense)null);

            // Act
            await _expenseService.DeleteExpenseAsync(999);

            // Assert
            _mockExpenseRepository.Verify(repo => repo.DeleteExpenseAsync(It.IsAny<Expense>()), Times.Never);
        }

        [Fact]
        public async Task PayExpenseAsync_ExpenseShareNotFound_ReturnsFalse()
        {
            // Arrange
            _mockExpenseRepository.Setup(repo => repo.GetExpenseShareByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((ExpenseShare)null);

            // Act
            var result = await _expenseService.PayExpenseAsync(999, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PayExpenseAsync_ExpenseNotFound_ReturnsFalse()
        {
            // Arrange
            var expenseShare = new ExpenseShare { ExpenseId = 1, UserId = 1 };
            _mockExpenseRepository.Setup(repo => repo.GetExpenseShareByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expenseShare);
            _mockExpenseRepository.Setup(repo => repo.GetExpenseByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Expense)null);

            // Act
            var result = await _expenseService.PayExpenseAsync(1, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PayExpenseAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var expenseShare = new ExpenseShare { ExpenseId = 1, UserId = 1 };
            var expense = new Expense { PaidById = 1 };
            _mockExpenseRepository.Setup(repo => repo.GetExpenseShareByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expenseShare);
            _mockExpenseRepository.Setup(repo => repo.GetExpenseByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expense);
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _expenseService.PayExpenseAsync(1, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetUnpaidExpenseSharesForUserAsync_InvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            _mockExpenseRepository.Setup(repo => repo.GetUnpaidExpenseSharesForUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<ExpenseShare>());

            // Act
            var result = await _expenseService.GetUnpaidExpenseSharesForUserAsync(999);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetExpenseShareByIdAsync_InvalidExpenseShareId_ReturnsNull()
        {
            // Arrange
            _mockExpenseRepository.Setup(repo => repo.GetExpenseShareByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((ExpenseShare)null);

            // Act
            var result = await _expenseService.GetExpenseShareByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}
