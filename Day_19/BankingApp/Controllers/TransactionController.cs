using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] TransactionDto transactionDto)
    {
        try
        {
            if (transactionDto == null || transactionDto.Amount <= 0 || transactionDto.AccountId <= 0)
            {
                return BadRequest(new { message = "Invalid transaction data." });
            }
            var result = await _transactionService.DepositAsync(transactionDto.AccountId, transactionDto.Amount);
            if (result == null)
            {
                return NotFound(new { message = "Account not found." });
            }
            
            return Ok(new { message = "Amount Deposited Successfully", balance = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetBalance(int accountId)
    {
        try
        {
            if (accountId <= 0)
            {
                return BadRequest(new { message = "Invalid account ID." });
            }
            var balance = await _transactionService.GetBalanceAsync(accountId);
            if (balance == null)
            {
                return NotFound(new { message = "Account not found." });
            }
            return Ok(new { Balance = balance });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] TransactionDto transactionDto)
    {
        try
        {
            if (transactionDto == null || transactionDto.Amount <= 0 || transactionDto.AccountId <= 0)
            {
                return BadRequest(new { message = "Invalid transaction data." });
            }
            var result = await _transactionService.WithdrawAsync(transactionDto.AccountId, transactionDto.Amount);
            if (result == null)
            {
                return NotFound(new { message = "Account not found or insufficient funds." });
            }
            return Ok(new { message = "Amount Withdrawn Successfully", balance = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}