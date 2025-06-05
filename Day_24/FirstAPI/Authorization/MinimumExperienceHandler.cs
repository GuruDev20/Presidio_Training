using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class MinimumExperienceHandler : AuthorizationHandler<MinimumExperienceRequirement>
{
    private readonly IRepository<string,Appointment> _appointmentRepository;
    private readonly IRepository<int, Doctor> _doctorRepository;
    public MinimumExperienceHandler(IRepository<int, Doctor> doctorRepository, IRepository<string, Appointment> appointmentRepository)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumExperienceRequirement requirement)
    {
        var appointmentNumber = context.Resource as string;
        Console.WriteLine($"Appointment Number: {appointmentNumber}");
        if (string.IsNullOrEmpty(appointmentNumber))
        {
            context.Fail();
            return;
        }
        var email = context.User?.FindFirstValue(ClaimTypes.Email)
                ?? context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? context.User?.FindFirstValue("preferred_username");
        Console.WriteLine($"Email: {email}");
        if (string.IsNullOrEmpty(email))
        {
            context.Fail();
            return;
        }

        var doctors = await _doctorRepository.GetAll();
        var doctor = doctors.FirstOrDefault(d => d.Email == email);
        Console.WriteLine($"Doctor: {doctor?.Name}, Years of Experience: {doctor?.YearsOfExperience}");
        if (doctor == null || doctor.YearsOfExperience < requirement.MinimumExperience)
        {
            context.Fail();
            return;
        }
        var appointment = await _appointmentRepository.Get(appointmentNumber);
        if (appointment == null || appointment.DoctorId != doctor.Id)
        {
            context.Fail();
            return;
        }
        context.Succeed(requirement);
    }
}