using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.FeedbackDTOs;
using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [HttpGet("{feedbackId}")]
        public async Task<IActionResult> GetFeedbackAsync(Guid feedbackId)
        {
            var feedback = await _feedbackService.GetFeedbackAsync(feedbackId);
            var response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: feedback,
                message: "Get feedback success."
            );
            return Ok(response);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllFeedbacksByUserIdAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByUserIdAsync(userId, page, pageSize);

            var response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: feedbacks,
                message: "Get feedbacks success."
            );
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateFeedbackAsync([FromBody] PostFeedbackDto postFeedbackDto)
        {
            var result = await _feedbackService.CreateFeedbackAsync(postFeedbackDto);
            BaseResponseModel response;

            if (result == false)
            {
                response = new BaseResponseModel(
                statusCode: StatusCodes.Status400BadRequest,
                code: ResponseCodeConstants.BADREQUEST,
                data: null,
                message: "Create feedback fail."
                );
                return BadRequest(response);
            }

            response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Create feedback success."
                );
            return Ok(response);
        }
        /*[HttpPut]
        public async Task<IActionResult> UpdateFeedbackAsync([FromBody] PutFeedbackDto putFeedbackDto)
        {
            await _feedbackService.UpdateFeedbackAsync(putFeedbackDto);
            return NoContent();
        }*/
        /*[HttpDelete("{feedbackId}")]
        public async Task<IActionResult> DeleteFeedbackAsync(Guid feedbackId)
        {
            var result = await _feedbackService.DeleteFeedbackAsync(feedbackId);
            BaseResponseModel response;

            if (result == false)
            {
                response = new BaseResponseModel(
                statusCode: StatusCodes.Status404NotFound,
                code: ResponseCodeConstants.NOT_FOUND,
                data: null,
                message: "Delete feedback fail."
                );
                return NotFound(response);
            }

            response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Delete feedback success."
                );
            return Ok(response);
        }*/
        [HttpGet("all")]
        [Authorize(Roles = RoleConstants.Staff)]
        public async Task<IActionResult> GetAllFeedbacksAsync(int page = 1, int pageSize = 10)
        {
            var feedbacks = await _feedbackService.GetAllFeedbacksAsync(page, pageSize);
            var response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: feedbacks,
                message: "Get feedback list success."
            );
            return Ok(response);
        }
    }
}
