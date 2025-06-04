namespace FirstAPI.Test;
using Microsoft.EntityFrameworkCore;

using FirstAPI.Contexts;
public class Tests
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
    public async Task AddDoctorTest()
    {
        //arrange
        var email = "test@gmail.com";
        var password = System.Text.Encoding.UTF8.GetBytes("test123");
        var key = Guid.NewGuid().ToByteArray();

        var user = new User
        {
            Email = email,
            Password = password,
            HashKey = key,
            Role = "Doctor"
        };
        _context.Add(user);
        await _context.SaveChangesAsync();
        var doctor = new Doctor
        {
            Name = "test",
            YearsOfExperience = 2,
            Email = email
        };
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
        //action
        var result = await _doctorRepository.Add(doctor);
        //assert
        Assert.That(result,Is.Not.Null, "Doctor is not added");
        Assert.That(result.Id, Is.EqualTo(1));
    }

    [TestCase(1)]
    // [TestCase(2)]
    public async Task GetDoctorPassTest(int id)
    {
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
        //action
        var result = await _doctorRepository.Get(id);
        //assert
        Assert.That(result, Is.Not.Null, "Doctor is not added");
        Assert.That(result.Id, Is.EqualTo(id));

    }
    [TestCase(2)]
    public async Task GetDoctorExceptionTest(int id)
    {
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
        var ex=await Assert.ThrowsAsync<Exception>(async()=> await _doctorRepository.Get(id));
        // Assert.That(()=>_doctorRepository.Get(id),Throws.TypeOf<KeyNotFoundException>());
        Assert.That(ex.Message,Is.EqualTo("Doctor not found"));
    }
    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}