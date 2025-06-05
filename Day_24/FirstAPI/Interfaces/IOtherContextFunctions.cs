public interface IOtherContextFunctions
{
    public Task<ICollection<DoctorsBySpecialityResponseDTO>> GetDoctorBySpeciality(string speciality);
}