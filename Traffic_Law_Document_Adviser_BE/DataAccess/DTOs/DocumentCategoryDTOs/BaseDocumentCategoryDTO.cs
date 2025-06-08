using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.DTOs.DocumentCategoryDTOs
{
    public class BaseDocumentCategoryDTO
    {
        [JsonIgnore]
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        public string? LastUpdatedBy { get; set; }
        [JsonIgnore]
        public string? DeletedBy { get; set; }
        [JsonIgnore]
        public DateTime? CreatedTime { get; set; }
        [JsonIgnore]
        public DateTime? LastUpdatedTime { get; set; }
        [JsonIgnore]
        public DateTime? DeletedTime { get; set; }
    }
}
