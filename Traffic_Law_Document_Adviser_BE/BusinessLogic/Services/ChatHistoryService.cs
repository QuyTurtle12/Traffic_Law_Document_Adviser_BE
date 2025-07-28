using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.DTOs.ChatHistoryDTOs;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;

namespace BusinessLogic.Services
{
    public class ChatHistoryService : IChatHistoryService
    {
        private readonly IMapper _mapper;
        private readonly IUOW _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _http;

        public ChatHistoryService(IMapper mapper, IUOW uow, IConfiguration configuration, HttpClient http)
        {
            _mapper = mapper;
            _unitOfWork = uow;
            _configuration = configuration;
            _http = http;
        }
        public async Task<Guid> CreateChatHistoryAsync(PostChatHistoryDto postChatHistoryDto, string modelName)
        {
            if (modelName == "law-document")
            {
                postChatHistoryDto.Answer = await AnswerFromMyLLM(postChatHistoryDto.Question);
            }
            else if (modelName == "gemini-2.5-pro")
            {
                postChatHistoryDto.Answer = await AnswerFromGemini(postChatHistoryDto.Question);
            }
            ChatHistory newChatHistory = _mapper.Map<ChatHistory>(postChatHistoryDto);
            newChatHistory.CreatedTime = DateTime.Now;
            Guid id = newChatHistory.Id;

            try {
                await _unitOfWork.GetRepository<ChatHistory>().InsertAsync(newChatHistory);
                await _unitOfWork.SaveAsync();
            }catch (Exception)
            {
                return Guid.Empty;
            }
            return id;
        }
        public async Task<bool> DeleteChatHistoryAsync(Guid chatId)
        {
            ChatHistory? chatHistory = await _unitOfWork.GetRepository<ChatHistory>()
                .Entities
                .Where(c => Guid.Equals(c.Id, chatId))
                .FirstOrDefaultAsync();

            if (chatHistory == null) return false;

            _unitOfWork.GetRepository<ChatHistory>().Delete(chatHistory);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<GetChatHistoryDto> GetChatHistoryAsync(Guid chatId)
        {
            ChatHistory? chatHistory = await _unitOfWork.GetRepository<ChatHistory>()
               .Entities
               .Where(c => Guid.Equals(c.Id, chatId))
               .FirstOrDefaultAsync();

            return chatHistory == null ? null : _mapper.Map<GetChatHistoryDto>(chatHistory);
        }
        public async Task<IEnumerable<GetChatHistoryDto>> GetChatHistoryListAsync(Guid userId, int page, int pageSize)
        {
            IEnumerable<ChatHistory> chatHistoryList = await _unitOfWork.GetRepository<ChatHistory>()
                .Entities
                .Where(c => Guid.Equals(c.UserId, userId))
                .OrderByDescending(c => c.CreatedTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            return _mapper.Map<IEnumerable<GetChatHistoryDto>>(chatHistoryList);
        }
        private async Task<string> AnswerFromGemini(string question)
        {
            using var httpClient = new HttpClient();

            string apiKey = _configuration["Gemini:Key"];
            string endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent";

            string prompt = "You are a Vietnamese traffic law assistant. Answer the question in Vietnamese using only plain text. Do not use Markdown, bullets, asterisks (*), headings, or any special formatting.\n\n" +
                            "Question: " + question + "\n\n" +
                            "Answer:";

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                role = "user",
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        },
                generationConfig = new
                {
                    responseMimeType = "text/plain"
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{endpoint}?key={apiKey}", content);

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode}";
            }

            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = System.Text.Json.JsonDocument.Parse(responseString);
            try
            {
                return doc.RootElement
                          .GetProperty("candidates")[0]
                          .GetProperty("content")
                          .GetProperty("parts")[0]
                          .GetProperty("text")
                          .GetString() ?? "No answer returned.";
            }
            catch
            {
                return "Failed to parse response.";
            }
        }
        private async Task<string> AnswerFromMyLLM(string question)
        {
            //string apiUrl = "http://127.0.0.1:8000/ask";
            //string prompt = "Bạn là một trợ lý luật giao thông Việt Nam. Trả lời câu hỏi bằng tiếng Việt chỉ sử dụng văn bản thuần túy. Không sử dụng Markdown, dấu đầu dòng, dấu hoa thị (*) hay bất kỳ định dạng đặc biệt nào.\n\n" + "Câu hỏi: " + question + "\n\n" + "Trả lời:";

            //var response = await _http.PostAsJsonAsync(apiUrl, new { question = prompt });
            //var result = await response.Content.ReadAsStringAsync();

            return "Not implement yet";
        }
    }
}
