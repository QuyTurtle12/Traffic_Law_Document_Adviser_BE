using AutoMapper;
using DataAccess.DTOs.NewsDTOs;
using DataAccess.Entities;
using DataAccess.IRepositories;
using DataAccess.IServices;
using DataAccess.PaginatedList;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DataAccess.Services
{
    public class NewsService : INewsService
    {
        private readonly IGenericRepository<News> _newsRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiKey = "b1f8b0c7b1404dd99515fafeaa6c6632";

        public NewsService(
            IGenericRepository<News> newsRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<PaginatedList<GetNewsDTO>> GetPaginatedNewsAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = _newsRepository.Entities
                .Where(n => n.DeletedTime == null)
                .OrderByDescending(n => n.PublishedDate);

            var count = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();

            var dtoItems = _mapper.Map<List<GetNewsDTO>>(items);
            return new PaginatedList<GetNewsDTO>(dtoItems, count, pageIndex, pageSize);
        }

        public async Task<GetNewsDTO?> GetNewsByIdAsync(Guid id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null || news.DeletedTime != null) return null;
            return news == null ? null : _mapper.Map<GetNewsDTO>(news);
        }

        public async Task<GetNewsDTO> CreateNewsAsync(AddNewsDTO addNewsDTO)
        {
            if (!addNewsDTO.UserId.HasValue)
                throw new ArgumentException("UserId is required");

            var news = _mapper.Map<News>(addNewsDTO);
            news.CreatedTime = DateTime.UtcNow;
            news.UserId = addNewsDTO.UserId;

            await _newsRepository.InsertAsync(news);
            await _newsRepository.SaveAsync();

            return _mapper.Map<GetNewsDTO>(news);
        }

        public async Task<GetNewsDTO?> UpdateNewsAsync(Guid id, AddNewsDTO updateNewsDTO)
        {
            var existingNews = await _newsRepository.GetByIdAsync(id);
            if (existingNews == null) return null;

            _mapper.Map(updateNewsDTO, existingNews);
            existingNews.LastUpdatedTime = DateTime.UtcNow;

            await _newsRepository.UpdateAsync(existingNews);
            await _newsRepository.SaveAsync();

            return _mapper.Map<GetNewsDTO>(existingNews);
        }

        public async Task<bool> DeleteNewsAsync(Guid id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return false;

            news.DeletedTime = DateTime.UtcNow;
            news.DeletedBy = "System"; 

            await _newsRepository.UpdateAsync(news);
            await _newsRepository.SaveAsync();

            return true;
        }

        public async Task<List<GetNewsDTO>> SyncNewsFromApiAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("x-api-key", ApiKey);

                // Call API to get news
                var response = await client.GetAsync("https://api.worldnewsapi.com/search-news?language=vi&text=luật%20giao%20thông");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiNewsResponse>(content);

                if (apiResponse?.news == null || !apiResponse.news.Any())
                    return new List<GetNewsDTO>();

                var updatedNews = new List<News>();

                foreach (var article in apiResponse.news)
                {
                    // Check if news already exists by embeddedNewsId
                    var existingNews = await _newsRepository.Entities
                        .FirstOrDefaultAsync(n => n.embeddedNewsId == article.id);

                    if (existingNews != null)
                    {
                        // Update existing news
                        existingNews.Title = article.title;
                        existingNews.Content = article.summary;
                        existingNews.Author = string.Join(",", article.authors ?? new List<string> { article.author }.Where(a => !string.IsNullOrEmpty(a)));
                        existingNews.ImageUrl = article.image;
                        existingNews.EmbeddedUrl = article.url;
                        existingNews.PublishedDate = DateTime.Parse(article.publish_date);
                        existingNews.LastUpdatedTime = DateTime.UtcNow;
                        existingNews.LastUpdatedBy = "System";

                        await _newsRepository.UpdateAsync(existingNews);
                        updatedNews.Add(existingNews);
                    }
                    else
                    {
                        // Create new news
                        var news = new News
                        {
                            Id = Guid.NewGuid(),
                            embeddedNewsId = article.id,
                            Title = article.title,
                            Content = article.summary,
                            Author = string.Join(",", article.authors ?? new List<string> { article.author }.Where(a => !string.IsNullOrEmpty(a))),
                            ImageUrl = article.image,
                            EmbeddedUrl = article.url,
                            PublishedDate = DateTime.Parse(article.publish_date),
                            CreatedTime = DateTime.UtcNow,
                            CreatedBy = "System"
                        };

                        await _newsRepository.InsertAsync(news);
                        updatedNews.Add(news);
                    }
                }

                await _newsRepository.SaveAsync();
                return _mapper.Map<List<GetNewsDTO>>(updatedNews);
            }
            catch (Exception ex)
            {
                // Log error details
                throw;
            }
        }

        private class ApiNewsResponse
        {
            public int offset { get; set; }
            public int number { get; set; }
            public int available { get; set; }
            public List<ApiNewsArticle> news { get; set; } = new();
        }

        private class ApiNewsArticle
        {
            public int id { get; set; }
            public string title { get; set; } = string.Empty;
            public string text { get; set; } = string.Empty;
            public string summary { get; set; } = string.Empty;
            public string url { get; set; } = string.Empty;
            public string image { get; set; } = string.Empty;
            public string publish_date { get; set; } = string.Empty;
            public string author { get; set; } = string.Empty;
            public List<string> authors { get; set; } = new();
            public string language { get; set; } = string.Empty;
            public string source_country { get; set; } = string.Empty;
        }
    }
}