using System.Text;
using AutoMapper;
using FirstAPI.Contexts;
using FirstAPI.Interfaces;
using FirstAPI.Repositories;
using FirstAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var key = Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = "External";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
    };
})
.AddCookie("External")
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.CallbackPath = "/signin-google";
    options.SaveTokens = true;
    options.Events.OnRemoteFailure = context =>
    {
        context.Response.Redirect("/error?failure=" + Uri.EscapeDataString(context.Failure?.Message ?? "Unknown"));
        context.HandleResponse(); 
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    opts.JsonSerializerOptions.WriteIndented = true;
                });

builder.Logging.AddLog4Net();

#region Repositories
builder.Services.AddTransient<IRepository<int, Doctor>, DoctorRepository>();
builder.Services.AddTransient<IRepository<int,Patient>,PatientRepository>();
builder.Services.AddTransient<IRepository<int, Speciality>, SpecialityRepository>();
builder.Services.AddTransient<IRepository<string,Appointment>, AppointmentRepository>();
builder.Services.AddTransient<IRepository<int, DoctorSpeciality>, DoctorSpecialityRepository>();
builder.Services.AddTransient<IRepository<string,User>, UserRepository>();
#endregion

#region Services
builder.Services.AddTransient<IDoctorService, DoctorService>();
builder.Services.AddTransient<IOtherContextFunctions, OtherFunctionalitiesImplementation>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IPatientService, PatientService>();
builder.Services.AddTransient<IAppointmentService, AppointmentService>();
#endregion

#region  Misc
builder.Services.AddAutoMapper(typeof(User));
// builder.Services.AddScoped<CustomExceptionFilter>();
#endregion


builder.Services.AddTransient<IAuthorizationHandler, MinimumExperienceHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ExperiencedDoctorOnly", policy =>
        policy.Requirements.Add(new MinimumExperienceRequirement(3)));
});

builder.Services.AddDbContext<ClinicContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();  