using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;

        public PhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string fileName)
        {
            var result = await _photoService.UploadImageAsync(file, fileName);
            BaseResponseModel response;

            if(result == null)
            {
                response = new BaseResponseModel(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.BADREQUEST,
                    data: result,
                    message: "Upload image fail."
                );
                return BadRequest(response);
            }

            response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Upload image success."
            );
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery] string url)
        {
            var (stream, contentType) = await _photoService.GetImageAsync(url);
            BaseResponseModel response;

            if (stream == null) 
            {
                response = new BaseResponseModel(
                    statusCode: StatusCodes.Status404NotFound,
                    code: ResponseCodeConstants.NOT_FOUND,
                    message: "Image not found."
                );
                return NotFound(response); 
            }
            // Convert the stream to base64
            using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var base64String = Convert.ToBase64String(memoryStream.ToArray());

            var imageData = new
            {
                ContentType = contentType,
                Base64Data = $"data:{contentType};base64,{base64String}"
            };

            response = new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: imageData,
                message: "Get image success."
            );
            return Ok(response);
        }
        [HttpDelete]
        public async Task<IActionResult> DelteImage([FromQuery] string url)
        {
            BaseResponseModel response;
            var result = await _photoService.DeleteImageAsync(url);

            if (result)
            {
                response = new BaseResponseModel(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    message: "Delete image success."
                );
                return Ok(response);
            }

            response = new BaseResponseModel(
                statusCode: StatusCodes.Status400BadRequest,
                code: ResponseCodeConstants.BADREQUEST,
                message: "Image delete failed."
            );
            return BadRequest(response);
        }
        [HttpGet("show")]
        public async Task<IActionResult> ShowImage([FromQuery] string url)
        {
            var (stream, contentType) = await _photoService.GetImageAsync(url);
            BaseResponseModel response;

            if (stream == null)
            {
                response = new BaseResponseModel(
                    statusCode: StatusCodes.Status404NotFound,
                    code: ResponseCodeConstants.NOT_FOUND,
                    message: "Image not found."
                );
                return NotFound(response);
            }

            return File(stream, contentType);
        }
    }
}
