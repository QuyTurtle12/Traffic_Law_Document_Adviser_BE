using AutoMapper;
using DataAccess.DTOs.DocumentTagMapDTOs;
using DataAccess.Entities;

namespace BusinessLogic.MappingProfiles
{
    public class DocumentTagMapProfile : Profile
    {
        public DocumentTagMapProfile()
        {
            CreateMap<AddDocumentTagMapDTO, DocumentTagMap>().ReverseMap();
            CreateMap<GetDocumentTagMapDTO, DocumentTagMap>().ReverseMap();
        }
    }
}
