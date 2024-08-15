using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Controllers;
using Xunit;

namespace UnitTests.Web.Controllers
{
    public class ExpenseControllerNegative
    {
        private readonly Mock<IExpenseService> _mockExpenseService;
        private readonly ExpenseController _controller;

        public ExpenseControllerNegative()
        {
            _mockExpenseService = new Mock<IExpenseService>();
            _controller = new ExpenseController(_mockExpenseService.Object);
        }

        [Fact]
        public async Task AddExpense_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Description", "Description is required");
            var expense = new Expense(); // This will trigger ModelState.IsValid to be false

            // Act
            var result = await _controller.AddExpense(expense);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetExpense_ExpenseNotFound_ReturnsNotFound()
        {
            // Arrange
            var expenseId = 1;
            _mockExpenseService.Setup(repo => repo.GetExpenseByIdAsync(expenseId))
                .ReturnsAsync((Expense)null);

            // Act
            var result = await _controller.GetExpense(expenseId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

       
        
        [Fact]
        public async Task GetUnpaidExpenseSharesForUser_NoUnpaidShares_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            _mockExpenseService.Setup(repo => repo.GetUnpaidExpenseSharesForUserAsync(userId))
                .ReturnsAsync(new List<ExpenseShare>());

            // Act
            var result = await _controller.GetUnpaidExpenseSharesForUser(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

