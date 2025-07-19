using DataAccess.DTOs.ChatHistoryDTOs;
using DataAccess.Entities;
using AutoMapper;

namespace BusinessLogic.MappingProfiles
{
    public class ChatHistoryProfile : Profile
    {
        public ChatHistoryProfile()
        {
            CreateMap<ChatHistory, GetChatHistoryDto>().ReverseMap();
            CreateMap<ChatHistory, PostChatHistoryDto>().ReverseMap();
        }
    }
}
