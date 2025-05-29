using FirstAPI.Contexts;
using FirstAPI.Misc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options=>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddTransient<IRepository<int, Doctor>, DoctorRepository>();
builder.Services.AddTransient<IRepository<int,Patient>,PatientRepository>();
builder.Services.AddTransient<IRepository<int, Speciality>, SpecialityRepository>();
builder.Services.AddTransient<IRepository<string,Appointment>, AppointmentRepository>();
builder.Services.AddTransient<IRepository<int, DoctorSpeciality>, DoctorSpecialityRepository>();

builder.Services.AddTransient<DoctorMapper>();
builder.Services.AddTransient<SpecialityMapper>();
builder.Services.AddTransient<IDoctorService, DoctorServiceWitTransaction>();
builder.Services.AddTransient<IOtherContextFunctions, OtherFunctionalitiesImplementation>();

builder.Services.AddDbContext<ClinicContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.Run();  