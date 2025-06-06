using AutoMapper;
using DataAccess.DTOs.DocumentCategoryDTOs;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.MappingProfiles
{
    public class DocumentCategoryProfile : Profile
    {
        public DocumentCategoryProfile()
        {
            CreateMap<DocumentCategory, GetDocumentCategoryDTO>().ReverseMap();
            CreateMap<DocumentCategory, AddDocumentCategoryDTO>().ReverseMap();
            CreateMap<DocumentCategory, UpdateDocumentCategoryDTO>().ReverseMap();
        }
    }
}
