using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class SampleController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Doctor")]
    public IActionResult GetGreet()
    {
        return Ok("Hello World");
    }
}