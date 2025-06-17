using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.ChatHistoryDTOs;
using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatHistoryController : ControllerBase
    {
        private readonly IChatHistoryService _chatHistoryService;
        public ChatHistoryController(IChatHistoryService chatHistoryService)
        {
            _chatHistoryService = chatHistoryService;
        }
        [HttpGet("search/{userId}")]
        public async Task<IActionResult> GetChatHistoryListAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            var chatHistoryList = await _chatHistoryService.GetChatHistoryListAsync(userId, page, pageSize);

            var response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: chatHistoryList,
                message: "Get chat history list success."
            );
            return Ok(response);
        }
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatHistoryAsync(Guid chatId)
        {
            var chatHistory = await _chatHistoryService.GetChatHistoryAsync(chatId);
            BaseResponseModel response;

            if (chatHistory == null)
            {
                response = new BaseResponseModel(
                statusCode: StatusCodes.Status404NotFound,
                code: ResponseCodeConstants.NOT_FOUND,
                data: null,
                message: "Get chat history fail."
                );
                return NotFound(response);
            }

            response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: chatHistory,
                message: "Get chat history success."
                );
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateChatHistoryAsync([FromBody] PostChatHistoryDto postChatHistoryDto)
        {
            var result = await _chatHistoryService.CreateChatHistoryAsync(postChatHistoryDto);
            BaseResponseModel response;

            if (result == false)
            {
                response = new BaseResponseModel(
                statusCode: StatusCodes.Status400BadRequest,
                code: ResponseCodeConstants.BADREQUEST,
                data: null,
                message: "Create chat history fail."
                );
                return BadRequest(response);
            }

            response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Create chat history success."
                );
            return Ok(response);
        }
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChatHistoryAsync(Guid chatId)
        {
            var result = await _chatHistoryService.DeleteChatHistoryAsync(chatId);
            BaseResponseModel response;

            if (result == false)
            {
                response = new BaseResponseModel(
                statusCode: StatusCodes.Status404NotFound,
                code: ResponseCodeConstants.NOT_FOUND,
                data: null,
                message: "Delete chat history fail."
                );
                return NotFound(response);
            }

            response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Delete chat history success."
                );
            return Ok(response);
        }
    }
}
