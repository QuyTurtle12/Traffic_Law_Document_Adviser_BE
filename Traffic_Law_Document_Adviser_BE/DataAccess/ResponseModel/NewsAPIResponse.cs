using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAccess.ResponseModel
{
    public class WorldNewsApiResponse
    {
        [JsonPropertyName("articles")]
        public List<WorldNewsArticle> Articles { get; set; } = new();

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("total_hits")]
        public int TotalHits { get; set; }
    }

    public class WorldNewsArticle
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;

        [JsonPropertyName("publish_date")]
        public DateTime PublishDate { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;
    }
}
