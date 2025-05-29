using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.AccountHolderName) || request.InitialBalance < 0)
        {
            return BadRequest("Invalid account creation request.");
        }

        var account = await _accountService.CreateAccountAsync(request);
        if (account != null)
        {
            return Created(string.Empty, new
            {
                message = "Account created successfully.",
                data = account
            });

        }
        return BadRequest("Failed to create account.");
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateAccountRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.AccountNumber))
        {
            return BadRequest("Invalid account update request.");
        }

        var result = await _accountService.UpdateAccountAsync(request);
        if (result != null)
        {
            return Ok(new
            {
                message = "Account updated successfully.",
                data = result
            });
        }
        return NotFound("Account not found.");
    }

    [HttpGet("{accountNumber}")]
    public async Task<IActionResult> GetAccount(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber))
        {
            return BadRequest("Account number is required.");
        }

        var account = await _accountService.GetAccountByNumberAsync(accountNumber);
        if (account != null)
        {
            return Ok(account);
        }
        return NotFound("Account not found.");
    }
}