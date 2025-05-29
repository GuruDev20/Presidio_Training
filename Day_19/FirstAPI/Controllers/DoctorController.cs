using System.Threading.Tasks;
using FirstAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }
    [HttpPost]
    public async Task<IActionResult> PostDoctor([FromBody] DoctorAddRequestDto doctor)
    {
        try
        {
            var newDoctor = await _doctorService.AddDoctor(doctor);
            if (newDoctor == null)
            {
                return BadRequest("Failed to add the doctor.");
            }
            return Created("", newDoctor);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorsBySpecialityResponseDTO>>> GetDoctors(string speciality)
    {
        var result = await _doctorService.GetDoctorsBySpecialityAsync(speciality);
        return Ok(result);
    }
}