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

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto createAccountDto)
    {
        try
        {
            if (createAccountDto == null)
            {
                return BadRequest(new { message = "Invalid account data." });
            }

            var account = await _accountService.CreateAccountAsync(createAccountDto);
            return Created("", account);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAccount(int accountId)
    {
        try
        {
            if (accountId <= 0)
            {
                return BadRequest(new { message = "Invalid account ID." });
            }
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return NotFound(new { message = "Account not found." });
            }
            return Ok(account);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}