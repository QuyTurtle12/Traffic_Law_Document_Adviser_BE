using DataAccess.DTOs.LawDocumentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.DocumentCategoryDTOs
{
    public class AddDocumentCategoryDTO : BaseDocumentCategoryDTO
    {
        public string? Title { get; set; }
        public string? DocumentCode { get; set; }
        public Guid? CategoryId { get; set; }
        public string? FilePath { get; set; }
        public string? LinkPath { get; set; }
        public bool ExpertVerification { get; set; } = false;
    }
}
