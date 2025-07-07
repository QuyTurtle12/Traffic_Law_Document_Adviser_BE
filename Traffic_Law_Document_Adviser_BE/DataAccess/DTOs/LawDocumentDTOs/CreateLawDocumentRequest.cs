using DataAccess.DTOs.DocumentTagMapDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.LawDocumentDTOs
{
    public class CreateLawDocumentRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string DocumentCode { get; set; }

        public Guid? CategoryId { get; set; }

        public IEnumerable<AddDocumentTagMapDTO>? TagList { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
