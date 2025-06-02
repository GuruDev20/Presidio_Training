using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FAQController : ControllerBase
{
    private readonly IFAQService _faqService;

    public FAQController(IFAQService faqService)
    {
        _faqService = faqService;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask([FromQuery] string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return BadRequest("Question cannot be empty.");
        }
        var answer = await _faqService.GetAnswerAsync(question);
        return Ok(new { question, answer });
    }
}
