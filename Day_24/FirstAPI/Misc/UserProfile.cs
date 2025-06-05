using AutoMapper;
using FirstAPI.Models.DTOs;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<DoctorAddRequestDto, User>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<User, DoctorAddRequestDto>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email));

        CreateMap<PatientAddRequestDto, User>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<User, PatientAddRequestDto>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email));
        
        CreateMap<PatientAddRequestDto, Patient>();
    }
}