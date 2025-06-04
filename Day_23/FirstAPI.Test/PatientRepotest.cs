using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAPI.Test
{
    public class PatientRepositoryTest
    {
        private ClinicContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestDb")
                            .Options;
            _context = new ClinicContext(options);
        }

        [Test]
        public async Task AddPatientTest()
        {
            // Arrange
            var user = new User
            {
                Email = "patient1@gmail.com",
                Password = System.Text.Encoding.UTF8.GetBytes("password123"),
                HashKey = Guid.NewGuid().ToByteArray(),
                Role = "Patient"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var patient = new Patient
            {
                FullName = "Test Patient",
                Age = 30,
                Email = user.Email,
                ContactNumber = "1234567890",
                Status = "Active",
                User = user
            };

            IRepository<int, Patient> patientRepository = new PatientRepository(_context);

            // Act
            var result = await patientRepository.Add(patient);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.FullName, Is.EqualTo("Test Patient"));
        }

        [Test]
        public async Task GetPatientById_Success()
        {
            // Arrange
            var patient = new Patient
            {
                FullName = "Existing Patient",
                Age = 40,
                Email = "existing@user.com",
                ContactNumber = "9876543210",
                Status = "Active"
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            IRepository<int, Patient> patientRepository = new PatientRepository(_context);

            // Act
            var result = await patientRepository.Get(patient.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(patient.Id));
            Assert.That(result.FullName, Is.EqualTo("Existing Patient"));
        }

        [Test]
        public async Task GetPatientById_ShouldThrowIfNotFound()
        {
            // Arrange
            IRepository<int, Patient> patientRepository = new PatientRepository(_context);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await patientRepository.Get(999));
            Assert.That(ex.Message, Is.EqualTo("Patient not found"));
        }

        [Test]
        public async Task GetAllPatients_ShouldReturnList()
        {
            // Arrange
            var patients = new List<Patient>
            {
                new Patient { FullName = "Patient One", Age = 25, Email = "one@test.com", ContactNumber = "111111", Status = "Active" },
                new Patient { FullName = "Patient Two", Age = 35, Email = "two@test.com", ContactNumber = "222222", Status = "Active" }
            };

            _context.Patients.AddRange(patients);
            await _context.SaveChangesAsync();

            IRepository<int, Patient> patientRepository = new PatientRepository(_context);

            // Act
            var result = await patientRepository.GetAll();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.FullName == "Patient One"));
        }

        [Test]
        public async Task GetAllPatients_ShouldThrowIfEmpty()
        {
            // Arrange
            IRepository<int, Patient> patientRepository = new PatientRepository(_context);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await patientRepository.GetAll());
            Assert.That(ex.Message, Is.EqualTo("No patients found"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
