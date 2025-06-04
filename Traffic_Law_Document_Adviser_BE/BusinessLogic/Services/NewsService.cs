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
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", ApiKey);

            var response = await client.GetAsync("https://api.worldnewsapi.com/search-news?language=vi&text=luật%20giao%20thông");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);

            if (apiResponse?.News == null) return new List<GetNewsDTO>();

            var newsList = apiResponse.News.Select(article => new News
            {
                Id = Guid.NewGuid(),
                Title = article.Title,
                Content = article.Text,
                Author = article.Author,
                ImageUrl = article.Image,
                EmbeddedUrl = article.Url,
                PublishedDate = DateTime.Parse(article.PublishDate),
                CreatedTime = DateTime.UtcNow
            }).ToList();

            foreach (var news in newsList)
            {
                await _newsRepository.InsertAsync(news);
            }
            await _newsRepository.SaveAsync();

            return _mapper.Map<List<GetNewsDTO>>(newsList);
        }

        private class ApiResponse
        {
            public int Offset { get; set; }
            public int Number { get; set; }
            public int Available { get; set; }
            public List<ApiNewsArticle> News { get; set; } = new();
        }

        private class ApiNewsArticle
        {
            public string Title { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
            public string Summary { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Image { get; set; } = string.Empty;
            public string PublishDate { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
            public List<string> Authors { get; set; } = new();
            public string Language { get; set; } = string.Empty;
        }
    }
}