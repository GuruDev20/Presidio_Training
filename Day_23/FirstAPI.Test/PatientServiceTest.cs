using AutoMapper;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using FirstAPI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestFixture]
public class PatientServiceTest
{
    private Mock<IRepository<int, Patient>> _patientRepositoryMock;
    private Mock<IEncryptionService> _encryptionServiceMock;
    private Mock<IRepository<string, User>> _userRepositoryMock;
    private Mock<IMapper> _mapperMock;

    private IPatientService _patientService;

    [SetUp]
    public void Setup()
    {
        _patientRepositoryMock = new Mock<IRepository<int, Patient>>();
        _encryptionServiceMock = new Mock<IEncryptionService>();
        _userRepositoryMock = new Mock<IRepository<string, User>>();
        _mapperMock = new Mock<IMapper>();

        _patientService = new PatientService(
            _patientRepositoryMock.Object,
            _encryptionServiceMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Test]
    public async Task AddPatient_ShouldAddPatientSuccessfully()
    {
        var dto = new PatientAddRequestDto
        {
            Email = "patient@example.com",
            Password = "securepass"
        };

        _userRepositoryMock.Setup(x => x.Get(dto.Email.ToLower())).ReturnsAsync((User)null);
        _mapperMock.Setup(m => m.Map<PatientAddRequestDto, User>(dto)).Returns(new User());
        _encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptionModel>()))
                              .ReturnsAsync(new EncryptionResponse { EncryptedData = "encrypted", HashKey = "key" });
        _userRepositoryMock.Setup(x => x.Add(It.IsAny<User>())).ReturnsAsync(new User());
        _mapperMock.Setup(m => m.Map<PatientAddRequestDto, Patient>(dto)).Returns(new Patient());
        _patientRepositoryMock.Setup(x => x.Add(It.IsAny<Patient>())).ReturnsAsync(new Patient { Id = 1 });

        var result = await _patientService.AddPatient(dto);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public void AddPatient_ShouldThrowIfEmailExists()
    {
        var dto = new PatientAddRequestDto
        {
            Email = "patient@example.com",
            Password = "securepass"
        };

        _userRepositoryMock.Setup(x => x.Get(dto.Email.ToLower())).ReturnsAsync(new User());

        var ex = Assert.ThrowsAsync<Exception>(async () => await _patientService.AddPatient(dto));
        Assert.That(ex.Message, Is.EqualTo("A user with this email already exists."));
    }
}
