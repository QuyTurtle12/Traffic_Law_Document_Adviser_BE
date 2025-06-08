using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.NewsDTOs
{
    public class BaseNewsDTO
    {
        public string? Title { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
    }
}
