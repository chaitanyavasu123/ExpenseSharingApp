using BusinessLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using Web.Controllers;
using Xunit;

namespace UnitTests.Web.Controllers
{
    public class ExpenseControllerTests
    {
        private readonly Mock<IExpenseService> _expenseServiceMock;
        private readonly ExpenseController _expenseController;

        public ExpenseControllerTests()
        {
            _expenseServiceMock = new Mock<IExpenseService>();
            _expenseController = new ExpenseController(_expenseServiceMock.Object);
        }

        [Fact]
        public async Task AddExpense_ValidExpense_ReturnsOk()
        {
            // Arrange
            var expense = new Expense { ExpenseId = 1, Amount = 100 };

            // Act
            var result = await _expenseController.AddExpense(expense);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _expenseServiceMock.Verify(x => x.AddExpenseAsync(expense), Times.Once);
        }

        [Fact]
        public async Task GetExpense_ExistingExpenseId_ReturnsOkWithExpense()
        {
            // Arrange
            int expenseId = 1;
            var expense = new Expense { ExpenseId = expenseId, Amount = 100 };
            _expenseServiceMock.Setup(x => x.GetExpenseByIdAsync(expenseId)).ReturnsAsync(expense);

            // Act
            var result = await _expenseController.GetExpense(expenseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expense, okResult.Value);
        }

        [Fact]
        public async Task GetExpense_NonExistingExpenseId_ReturnsNotFound()
        {
            // Arrange
            int expenseId = 1;
            _expenseServiceMock.Setup(x => x.GetExpenseByIdAsync(expenseId)).ReturnsAsync((Expense)null);

            // Act
            var result = await _expenseController.GetExpense(expenseId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteExpense_ValidExpenseId_ReturnsNoContent()
        {
            // Arrange
            int expenseId = 1;

            // Act
            var result = await _expenseController.DeleteExpense(expenseId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            _expenseServiceMock.Verify(x => x.DeleteExpenseAsync(expenseId), Times.Once);
        }

       
    }
}

