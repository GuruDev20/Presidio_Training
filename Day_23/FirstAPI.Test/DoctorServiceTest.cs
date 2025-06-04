using AutoMapper;
using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using FirstAPI.Repositories;
using FirstAPI.Services;
using FirstAPI.Misc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class DoctorServiceTest
{
    private Mock<IRepository<int, Doctor>> _doctorRepositoryMock;
    private Mock<IRepository<int, Speciality>> _specialityRepositoryMock;
    private Mock<IRepository<int, DoctorSpeciality>> _doctorSpecialityRepositoryMock;
    private Mock<IRepository<string, User>> _userRepositoryMock;
    private Mock<IOtherContextFunctions> _otherContextFunctionsMock;
    private Mock<IEncryptionService> _encryptionServiceMock;
    private Mock<IMapper> _mapperMock;

    private IDoctorService _doctorService;

    [SetUp]
    public void Setup()
    {
        _doctorRepositoryMock = new Mock<IRepository<int, Doctor>>();
        _specialityRepositoryMock = new Mock<IRepository<int, Speciality>>();
        _doctorSpecialityRepositoryMock = new Mock<IRepository<int, DoctorSpeciality>>();
        _userRepositoryMock = new Mock<IRepository<string, User>>();
        _otherContextFunctionsMock = new Mock<IOtherContextFunctions>();
        _encryptionServiceMock = new Mock<IEncryptionService>();
        _mapperMock = new Mock<IMapper>();

        _doctorService = new DoctorService(
            _doctorRepositoryMock.Object,
            _specialityRepositoryMock.Object,
            _doctorSpecialityRepositoryMock.Object,
            _otherContextFunctionsMock.Object,
            _encryptionServiceMock.Object,
            _mapperMock.Object,
            _userRepositoryMock.Object);
    }

    [Test]
    public async Task AddDoctor_ShouldAddDoctorSuccessfully()
    {
        var dto = new DoctorAddRequestDto
        {
            Email = "doc@example.com",
            Password = "secret",
            Specialities = new List<SpecialityAddRequestDTO> {
                new SpecialityAddRequestDTO { Name = "Cardiology" }
            }
        };

        _userRepositoryMock.Setup(x => x.Get(dto.Email.ToLower())).ReturnsAsync((User)null);
        _mapperMock.Setup(m => m.Map<DoctorAddRequestDto, User>(dto)).Returns(new User());
        _encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptionModel>()))
                              .ReturnsAsync(new EncryptionResponse { EncryptedData = "encrypted", HashKey = "key" });
        _userRepositoryMock.Setup(x => x.Add(It.IsAny<User>())).ReturnsAsync(new User());
        _doctorRepositoryMock.Setup(x => x.Add(It.IsAny<Doctor>())).ReturnsAsync(new Doctor { Id = 1 });
        _specialityRepositoryMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Speciality>());
        _specialityRepositoryMock.Setup(x => x.Add(It.IsAny<Speciality>()))
                                 .ReturnsAsync(new Speciality { Id = 10 });
        _doctorSpecialityRepositoryMock.Setup(x => x.Add(It.IsAny<DoctorSpeciality>()))
                                       .ReturnsAsync(new DoctorSpeciality());

        var result = await _doctorService.AddDoctor(dto);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(1));
    }

    [Test]
    public void AddDoctor_ShouldThrowIfEmailExists()
    {
        var dto = new DoctorAddRequestDto
        {
            Email = "doc@example.com",
            Password = "secret"
        };

        _userRepositoryMock.Setup(x => x.Get(dto.Email.ToLower())).ReturnsAsync(new User());

        var ex = Assert.ThrowsAsync<Exception>(async () => await _doctorService.AddDoctor(dto));
        Assert.That(ex.Message, Is.EqualTo("A user with this email already exists."));
    }

    [Test]
    public async Task GetDoctorsBySpeciality_ReturnsExpectedList()
    {
        _otherContextFunctionsMock.Setup(x => x.GetDoctorBySpeciality("Cardiology"))
                                  .ReturnsAsync(new List<DoctorsBySpecialityResponseDTO>
                                  {
                                      new DoctorsBySpecialityResponseDTO { Dname = "Dr. A", Yoe = 5, Id = 1 }
                                  });

        var result = await _doctorService.GetDoctorsBySpeciality("Cardiology");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
    }
}
