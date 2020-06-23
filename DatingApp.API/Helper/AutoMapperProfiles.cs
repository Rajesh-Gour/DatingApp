using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom( src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.Age , 
            opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<User, UserForDetailedDto>()
            .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom( src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.Age , 
            opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotosForDetailedDto>();

            CreateMap<UserForUpdateDto,User>();
        
            CreateMap<PhotoForCreationDto,Photo>();

            CreateMap<Photo,PhotosForReturnDto>();

            CreateMap<UserForRegisterDto, User>();

            CreateMap<MessageForCreationDto,Message>().ReverseMap();
            CreateMap<Message, MessageToReturn>()
            .ForMember(m => m.SenderPhotoUrl,opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain == true).Url))
            .ForMember(m => m.RecipientPhotoUrl,opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain == true).Url));
        }
    }
}