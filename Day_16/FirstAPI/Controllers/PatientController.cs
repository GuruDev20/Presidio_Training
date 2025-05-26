using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    static List<Patient> patients = new List<Patient>
    {
        new Patient{Id=101,FullName="Mikey",Age=30,Gender="Male",Address="123 Main St",PhoneNumber="123-456-7890",PatientNotes="No known allergies"},
        new Patient{Id=102,FullName="Sarah",Age=28,Gender="Female",Address="456 Elm St",PhoneNumber="987-654-3210",PatientNotes="Asthma"},
    };

    [HttpGet]
    public ActionResult<IEnumerable<Patient>> GetAllPatients()
    {
        return Ok(patients);
    }
    [HttpPost]
    public ActionResult<Patient> CreatePatient([FromBody] Patient newPatient)
    {
        if (newPatient == null)
        {
            return BadRequest("Patient data is null");
        }
        newPatient.Id = patients.Max(p => p.Id) + 1;
        patients.Add(newPatient);
        return Created("",newPatient);
    }
    [HttpPut("{id}")]
    public ActionResult UpdatePatient(int id, [FromBody] Patient updatedPatient)
    {
        if (updatedPatient == null || updatedPatient.Id != id)
        {
            return BadRequest("Patient data is invalid");
        }
        var existingPatient = patients.FirstOrDefault(p => p.Id == id);
        if (existingPatient == null)
        {
            return NotFound();
        }
        existingPatient.FullName = updatedPatient.FullName;
        existingPatient.Age = updatedPatient.Age;
        existingPatient.Gender = updatedPatient.Gender;
        existingPatient.Address = updatedPatient.Address;
        existingPatient.PhoneNumber = updatedPatient.PhoneNumber;
        existingPatient.PatientNotes = updatedPatient.PatientNotes;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult DeletePatient(int id)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound();
        }
        patients.Remove(patient);
        return NoContent();
    }
}