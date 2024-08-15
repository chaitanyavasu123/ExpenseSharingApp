using AutoMapper.Execution;
using BusinessLogic.Services;
using DataAccess.Repositories;
using Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BusinessLogic.Services
{
    public class ExpenseServiceTests
    {
        private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IGroupRepository> _groupRepositoryMock;
        private readonly ExpenseService _expenseService;

        public ExpenseServiceTests()
        {
            _expenseRepositoryMock = new Mock<IExpenseRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _groupRepositoryMock = new Mock<IGroupRepository>();
            _expenseService = new ExpenseService(_expenseRepositoryMock.Object, _userRepositoryMock.Object, _groupRepositoryMock.Object);
        }

        [Fact]
        public async Task AddExpenseAsync_ValidExpense_ExpenseAddedAndUsersUpdated()
        {
            // Arrange
            var expense = new Expense { GroupId = 1, Amount = 100, PaidById = 1 };
            var group = new Group
            {
                GroupId = 1,
                Members = new List<GroupMember>
                {
                    new GroupMember { UserId = 1 },
                    new GroupMember { UserId = 2 },
                    new GroupMember { UserId = 3 }
                }
            };
            var user1 = new User { UserId = 1, AmountOwed = 0, AmountOwedTo = 0 };
            var user2 = new User { UserId = 2, AmountOwed = 0, AmountOwedTo = 0 };
            var user3 = new User { UserId = 3, AmountOwed = 0, AmountOwedTo = 0 };

            _groupRepositoryMock.Setup(x => x.GetGroupByIdAsync(1)).ReturnsAsync(group);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user1);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(2)).ReturnsAsync(user2);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(3)).ReturnsAsync(user3);

            // Act
            await _expenseService.AddExpenseAsync(expense);

            // Assert
            Assert.Equal(34, user1.AmountOwed);
            Assert.Equal(33, user2.AmountOwed);
            Assert.Equal(33, user3.AmountOwed);
            Assert.Equal(66, user1.AmountOwedTo);

            _expenseRepositoryMock.Verify(x => x.AddExpenseAsync(expense), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateUserAsync(user1), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateUserAsync(user2), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateUserAsync(user3), Times.Once);
        }

        [Fact]
        public async Task DeleteExpenseAsync_ValidExpense_ExpenseDeletedAndUsersUpdated()
        {
            // Arrange
            var expense = new Expense { ExpenseId = 1, GroupId = 1, Amount = 100, PaidById = 1 };
            var group = new Group
            {
                GroupId = 1,
                Members = new List<GroupMember>
                {
                    new GroupMember { UserId = 1 },
                    new GroupMember { UserId = 2 },
                    new GroupMember { UserId = 3 }
                }
            };
            var user1 = new User { UserId = 1, AmountOwed = 34, AmountOwedTo = 66 };
            var user2 = new User { UserId = 2, AmountOwed = 33, AmountOwedTo = 0 };
            var user3 = new User { UserId = 3, AmountOwed = 33, AmountOwedTo = 0 };

            _expenseRepositoryMock.Setup(x => x.GetExpenseByIdAsync(1)).ReturnsAsync(expense);
            _groupRepositoryMock.Setup(x => x.GetGroupByIdAsync(1)).ReturnsAsync(group);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user1);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(2)).ReturnsAsync(user2);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(3)).ReturnsAsync(user3);

            // Act
            await _expenseService.DeleteExpenseAsync(1);

            // Assert
            Assert.Equal(0, user1.AmountOwed);
            Assert.Equal(0, user2.AmountOwed);
            Assert.Equal(0, user3.AmountOwed);
            Assert.Equal(0, user1.AmountOwedTo);

            _expenseRepositoryMock.Verify(x => x.DeleteExpenseAsync(expense), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateUserAsync(user1), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateUserAsync(user2), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateUserAsync(user3), Times.Once);
        }

        
    }
}
