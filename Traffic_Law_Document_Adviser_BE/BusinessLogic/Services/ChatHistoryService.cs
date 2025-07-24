using AutoMapper;
using Azure;
using BusinessLogic.IServices;
using DataAccess.DTOs.ChatHistoryDTOs;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Azure.AI.Inference;

namespace BusinessLogic.Services
{
    public class ChatHistoryService : IChatHistoryService
    {
        private readonly IMapper _mapper;
        private readonly IUOW _unitOfWork;
        private readonly IConfiguration _configuration;

        public ChatHistoryService(IMapper mapper, IUOW uow, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = uow;
            _configuration = configuration;
        }
        public async Task<Guid> CreateChatHistoryAsync(PostChatHistoryDto postChatHistoryDto)
        {
            postChatHistoryDto.Answer = await AnswerFromDeepseek(postChatHistoryDto.Question);
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
        private async Task<string> AnswerFromDeepseek(string question)
        {
            string prompt = "You are an Vietnamese traffic law assistant. Answer the question as best you can in Vietnamese. You just need to focus on the law and the main point.\n\n" +
                "Question: " + question + "\n\n" +
                "Answer:";
            var endpoint = new Uri("https://models.github.ai/inference");
            var credential = new AzureKeyCredential(_configuration["Github:Token"]);
            var model = "deepseek/DeepSeek-R1-0528";

            var client = new ChatCompletionsClient(
                endpoint,
                credential);

            var requestOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatRequestUserMessage(prompt),
                },
                MaxTokens = 2048,
                Model = model
            };

            Response<ChatCompletions> response = await client.CompleteAsync(requestOptions);
            string answer = response.Value.Choices[0].Message.Content;

            // Remove the <think> tag and its content
            int thinkEnd = answer.IndexOf("</think>");
            if (thinkEnd >= 0)
            {
                answer = answer.Substring(thinkEnd + "</think>".Length).TrimStart();
            }
            return answer;
        }
    }
}
