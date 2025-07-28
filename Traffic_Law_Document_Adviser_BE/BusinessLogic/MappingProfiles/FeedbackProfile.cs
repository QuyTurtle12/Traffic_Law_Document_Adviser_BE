using AutoMapper;
using DataAccess.DTOs.FeedbackDTOs;
using DataAccess.Entities;

namespace BusinessLogic.MappingProfiles
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<Feedback, GetFeedbackDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ReverseMap();
            CreateMap<Feedback, PostFeedbackDto>().ReverseMap();
            CreateMap<Feedback, PutFeedbackDto>().ReverseMap();
            
        }
    }
}
