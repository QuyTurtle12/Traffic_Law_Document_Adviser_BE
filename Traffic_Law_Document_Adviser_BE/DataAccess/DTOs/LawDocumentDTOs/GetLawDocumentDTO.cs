using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.LawDocumentDTOs
{
    public class GetLawDocumentDTO : BaseLawDocumentDTO
    {
        public string? CategoryName { get; set; }
    }
}
