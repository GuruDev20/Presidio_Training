using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpDelete("cancel/{appointmentNumber}")]
    [Authorize(Policy = "ExperiencedDoctorOnly")]
    [Authorize(Roles ="Doctor")]
    public async Task<IActionResult> CancelAppointment(string appointmentNumber)
    {
        try
        {
            var appointment = await _appointmentService.CancelAppointmentAsync(appointmentNumber);
            return Ok($"Appointment {appointment.AppointmentNumber} has been cancelled successfully.");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Appointment not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookAppointment([FromBody] AppointmentAddRequestDto addRequestDto)
    {
        try
        {
            var appointment = new Appointment
            {
                AppointmentNumber = addRequestDto.AppointmentNumber,
                DoctorId = addRequestDto.DoctorId,
                PatientId = addRequestDto.PatientId,
                AppointmentDate = addRequestDto.AppointmentDate
            };
            var createdAppointment = await _appointmentService.BookAppointmentAsync(appointment);
            return Created("", createdAppointment);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }

    [HttpGet("{appointmentNumber}")]
    public async Task<IActionResult> GetAppointment(string appointmentNumber)
    {
        try
        {
            var appointment = await _appointmentService.GetAppointmentAsync(appointmentNumber);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }
            return Ok(appointment);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }
}