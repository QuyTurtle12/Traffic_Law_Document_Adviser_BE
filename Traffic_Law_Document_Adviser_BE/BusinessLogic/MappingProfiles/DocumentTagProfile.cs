using AutoMapper;
using DataAccess.DTOs.DocumentTagDTOs;
using DataAccess.Entities;

namespace BusinessLogic.MappingProfiles
{
    public class DocumentTagProfile : Profile
    {
        public DocumentTagProfile() {
            CreateMap<DocumentTag, GetDocumentTagDTO>().ReverseMap();
            CreateMap<DocumentTag, AddDocumentTagDTO>().ReverseMap();
            CreateMap<DocumentTag, UpdateDocumentTagDTO>().ReverseMap();
        }
    }
}
