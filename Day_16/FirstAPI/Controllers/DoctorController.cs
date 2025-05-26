using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    static List<Doctor> doctors = new List<Doctor>
    {
        new Doctor{Id=101, Name="Mikey"},
        new Doctor{Id=102, Name="Prime"},
    };

    [HttpGet]
    public ActionResult<IEnumerable<Doctor>> GetDoctors()
    {
        return Ok(doctors);
    }

    [HttpPost]
    public ActionResult<Doctor> AddDoctor([FromBody] Doctor doctor)
    {
        if (doctor == null || string.IsNullOrEmpty(doctor.Name))
        {
            return BadRequest("Doctor data is invalid.");
        }
        doctors.Add(doctor);
        return CreatedAtAction(nameof(GetDoctors), new { id = doctor.Id }, doctor);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateDoctor(int id, [FromBody] Doctor updatedDoctor)
    {
        var doctor = doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound("Doctor not found.");
        }
        if (updatedDoctor == null || string.IsNullOrEmpty(updatedDoctor.Name))
        {
            return BadRequest("Updated doctor data is invalid.");
        }
        doctor.Name = updatedDoctor.Name;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteDoctor(int id)
    {
        var doctor = doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound("Doctor not found.");
        }
        doctors.Remove(doctor);
        return NoContent();
    }
}