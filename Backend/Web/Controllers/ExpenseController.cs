using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using System.Security.Claims;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddExpense([FromBody] Expense expense)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _expenseService.AddExpenseAsync(expense);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("{expenseId}")]
        [Authorize]
        public async Task<ActionResult<Expense>> GetExpense(int expenseId)
        {
            try
            {
                var expense = await _expenseService.GetExpenseByIdAsync(expenseId);
                if (expense == null)
                {
                    return NotFound();
                }

                return Ok(expense);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{expenseId}")]
        [Authorize]
        public async Task<ActionResult> DeleteExpense(int expenseId)
        {
            try
            {
                await _expenseService.DeleteExpenseAsync(expenseId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{expenseShareId}/pay")]
        [Authorize]
        public async Task<IActionResult> PayExpense(int expenseShareId)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value); // Get the current user's ID

            try
            {
                var result = await _expenseService.PayExpenseAsync(expenseShareId, userId);

                if (result)
                {
                    return Ok(new { message = "Payment successful" });
                }

                return BadRequest(new { message = "Payment failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("unpaid/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUnpaidExpenseSharesForUser(int userId)
        {
            var unpaidExpenseShares = await _expenseService.GetUnpaidExpenseSharesForUserAsync(userId);
            try
            {
                if (unpaidExpenseShares == null || !unpaidExpenseShares.Any())
                {
                    return NotFound();
                }

                return Ok(unpaidExpenseShares);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

    }
}
