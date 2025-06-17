using DataAccess.Constant;
using DataAccess.DTOs.NewsDTOs;
using DataAccess.IServices;
using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        /// <summary>
        /// Get paginated list of news
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Allow anonymous access to news details
        public async Task<IActionResult> GetNews([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _newsService.GetPaginatedNewsAsync(pageIndex, pageSize);
            return Ok(new BaseResponseModel<object>(
                StatusCodes.Status200OK,
                "SUCCESS",
                result,
                "Get news successfully"));
        }

        /// <summary>
        /// Get news by id
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous] // Allow anonymous access to news details
        public async Task<IActionResult> GetNewsById(Guid id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
                return NotFound(new BaseResponseModel<object>(
                    StatusCodes.Status404NotFound,
                    "NOT_FOUND",
                    null,
                    "News not found"));

            return Ok(new BaseResponseModel<object>(
                StatusCodes.Status200OK,
                "SUCCESS",
                news,
                "Get news successfully"));
        }

        /// <summary>
        /// Create a new news
        /// </summary>
        [HttpPost]
        [Authorize(Roles = RoleConstants.Staff)]
        public async Task<IActionResult> CreateNews([FromBody] AddNewsDTO addNewsDTO)
        {
            var result = await _newsService.CreateNewsAsync(addNewsDTO);
            return CreatedAtAction(nameof(GetNewsById),
                new { id = result.Id },
                new BaseResponseModel<object>(
                    StatusCodes.Status201Created,
                    "SUCCESS",
                    result,
                    "Create news successfully"));
        }

        /// <summary>
        /// Update an existing news
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.Staff)]
        public async Task<IActionResult> UpdateNews(Guid id, [FromBody] AddNewsDTO updateNewsDTO)
        {
            var result = await _newsService.UpdateNewsAsync(id, updateNewsDTO);
            if (result == null)
                return NotFound(new BaseResponseModel<object>(
                    StatusCodes.Status404NotFound,
                    "NOT_FOUND",
                    null,
                    "News not found"));

            return Ok(new BaseResponseModel<object>(
                StatusCodes.Status200OK,
                "SUCCESS",
                result,
                "Update news successfully"));
        }

        /// <summary>
        /// Delete a news
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.Staff)]
        public async Task<IActionResult> DeleteNews(Guid id)
        {
            var result = await _newsService.DeleteNewsAsync(id);
            if (!result)
                return NotFound(new BaseResponseModel<object>(
                    StatusCodes.Status404NotFound,
                    "NOT_FOUND",
                    null,
                    "News not found"));

            return NoContent();
        }

        // <summary>
        // Sync news from external API
        // </summary>
        [HttpPost("sync")]
        [Authorize(Roles = RoleConstants.Staff)]
        public async Task<IActionResult> SyncNews()
        {
            var result = await _newsService.SyncNewsFromApiAsync();
            return Ok(new BaseResponseModel<object>(
                StatusCodes.Status200OK,
                "SUCCESS",
                result,
                "Sync news successfully"));
        }
    }
}