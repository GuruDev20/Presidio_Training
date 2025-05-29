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
    [HttpPost("deposit/{accountNumber}")]
    public async Task<IActionResult> Deposit(string accountNumber, [FromQuery] decimal amount)
    {
        if (string.IsNullOrEmpty(accountNumber) || amount <= 0)
        {
            return BadRequest("Invalid deposit request.");
        }

        var result = await _transactionService.DepositAsync(accountNumber, amount);
        if (result)
        {
            return Ok($"Amount {amount} deposited successfully to {accountNumber}.");
        }
        return NotFound("Account not found.");
    }

    [HttpPost("withdraw/{accountNumber}")]
    public async Task<IActionResult> Withdraw(string accountNumber, [FromQuery] decimal amount)
    {
        if (string.IsNullOrEmpty(accountNumber) || amount <= 0)
        {
            return BadRequest("Invalid withdrawal request.");
        }

        var result = await _transactionService.WithdrawAsync(accountNumber, amount);
        if (result)
        {
            return Ok($"Amount {amount} withdrawn successfully from {accountNumber}.");
        }
        return NotFound("Account not found or insufficient balance.");
    }

    [HttpGet("history/{accountNumber}")]
    public async Task<IActionResult> GetTransactionHistory(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber))
        {
            return BadRequest("Account number is required.");
        }

        var history = await _transactionService.GetTransactionsByAccountAsync(accountNumber);
        if (history != null && history.Any())
        {
            return Ok(history);
        }
        return NotFound("No transactions found for the specified account.");
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.FromAccountNumber) || string.IsNullOrEmpty(request.ToAccountNumber) || request.Amount <= 0)
        {
            return BadRequest("Invalid transfer request.");
        }

        var result = await _transactionService.TransferAsync(request);
        if (result)
        {
            return Ok($"Amount {request.Amount} transferred successfully from {request.FromAccountNumber} to {request.ToAccountNumber}.");
        }
        return NotFound("Transfer failed. Check account numbers and balance.");
    }
}