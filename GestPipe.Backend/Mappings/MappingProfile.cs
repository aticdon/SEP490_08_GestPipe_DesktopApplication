using AutoMapper;
using GestPipe.Backend.Models;
using GestPipe.Backend.Models.DTOs;
using GestPipe.Backend.Models.DTOs.Auth;
using GestPipe.Backend.Models.DTOs.Profile;
using Google.Apis.Auth;
using System;

namespace GestPipe.Backend.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===== AUTH MAPPINGS =====
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<UserProfile, UserProfileDto>();

            // ✅ RegisterDto → User (với email normalization)
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower())) // ✅ Chuẩn hóa email
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.AccountStatus, opt => opt.MapFrom(src => "pending"))
                .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
                .ForMember(dest => dest.AuthProvider, opt => opt.Ignore())
                .ForMember(dest => dest.ProviderId, opt => opt.Ignore())
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore());

            // ✅ RegisterDto → UserProfile
            CreateMap<RegisterDto, UserProfile>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.EducationLevel, opt => opt.MapFrom(src => src.EducationLevel))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
                .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore());

            // ✅ Google Payload → User (với email normalization)
            CreateMap<GoogleJsonWebSignature.Payload, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower())) // ✅ Chuẩn hóa email
                .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.AccountStatus, opt => opt.MapFrom(src => "activeonline"))
                .ForMember(dest => dest.AuthProvider, opt => opt.MapFrom(src => "Google"))
                .ForMember(dest => dest.ProviderId, opt => opt.MapFrom(src => src.Subject))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Picture))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            // ✅ Google Payload → UserProfile
            CreateMap<GoogleJsonWebSignature.Payload, UserProfile>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.Picture))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Gender, opt => opt.Ignore())
                .ForMember(dest => dest.BirthDate, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Occupation, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.EducationLevel, opt => opt.Ignore());

            // ===== PROFILE MAPPINGS =====
            CreateMap<UpdateProfileDto, UserProfile>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.EducationLevel, opt => opt.MapFrom(src => src.EducationLevel))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
                .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}