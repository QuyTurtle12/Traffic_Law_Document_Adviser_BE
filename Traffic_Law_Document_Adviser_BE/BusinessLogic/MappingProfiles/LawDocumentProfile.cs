using AutoMapper;
using DataAccess.DTOs.LawDocumentDTOs;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.MappingProfiles
{
    public class LawDocumentProfile : Profile
    {
        public LawDocumentProfile()
        {
            CreateMap<LawDocument, GetLawDocumentDTO>().ReverseMap();
            CreateMap<LawDocument, AddLawDocumentDTO>().ReverseMap();
            CreateMap<LawDocument, UpdateLawDocumentDTO>().ReverseMap();
        }
    }
}
