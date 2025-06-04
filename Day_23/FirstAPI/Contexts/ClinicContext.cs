using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Contexts
{
    public class ClinicContext : DbContext
    {
        public ClinicContext(DbContextOptions<ClinicContext> options) : base(options)
        {

        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<DoctorSpeciality> DoctorSpecialities { get; set; }
        public DbSet<User> Users { get; set; }
        
        public async Task<List<DoctorsBySpecialityResponseDTO>> GetDoctorsBySpeciality(string speciality)
        {
            return await this.Set<DoctorsBySpecialityResponseDTO>()
                        .FromSqlInterpolated($"select * from proc_GetDoctorsBySpeciality({speciality})")
                        .ToListAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(u => u.Email)
                .HasConstraintName("FK_Patient_User")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(u => u.Email)
                .HasConstraintName("FK_Doctor_User")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasKey(app => app.AppointmentNumber).HasName("PK_AppointmentNumber");

            modelBuilder.Entity<Appointment>()
                .HasOne(app => app.Patient)
                .WithMany(pat => pat.Appointments)
                .HasForeignKey(app => app.PatientId)
                .HasConstraintName("FK_Appointment_Patient")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(app => app.Doctor)
                .WithMany(doc => doc.Appointments)
                .HasForeignKey(app => app.DoctorId)
                .HasConstraintName("FK_Appointment_Doctor")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSpeciality>()
                .HasKey(ds => ds.SerialNumber);

            modelBuilder.Entity<DoctorSpeciality>()
                .HasOne(ds => ds.Doctor)
                .WithMany(doc => doc.DoctorSpecialities)
                .HasForeignKey(ds => ds.DoctorId)
                .HasConstraintName("FK_Speciality_Doctor")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSpeciality>()
                .HasOne(ds => ds.Speciality)
                .WithMany(spec => spec.DoctorSpecialities)
                .HasForeignKey(ds => ds.SpecialityId)
                .HasConstraintName("FK_Speciality_Spec")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}