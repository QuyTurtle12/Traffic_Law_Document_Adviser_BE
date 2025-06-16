using BusinessLogic.IServices;
using BusinessLogic.Services;
using DataAccess.Constant;
using DataAccess.DTOs.DocumentCategoryDTOs;
using DataAccess.PaginatedList;
using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentCategoryController : Controller
    {
        private readonly IDocumentCategoryService _documentCategoryService;

        // Constructor
        public DocumentCategoryController(IDocumentCategoryService documentCategoryService)
        {
            _documentCategoryService = documentCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedCartsAsync(int pageIndex = 1, int pageSize = 10, Guid? idSearch = null, string? nameSearch = null)
        {
            PaginatedList<GetDocumentCategoryDTO> result = await _documentCategoryService.GetPaginatedDocumentCategoriesAsync(pageIndex, pageSize, idSearch, nameSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetDocumentCategoryDTO>>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: result,
                    message: "Document Category retrieved successfully."
                ));
        }

        [HttpPost]
        public async Task<IActionResult> PostDocumentCategoryAsync(AddDocumentCategoryDTO DocumentCategoryDTO)
        {
            if (DocumentCategoryDTO == null)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.BADREQUEST,
                data: null,
                    message: "Document Category data is required!"
                ));
            }

            try
            {
                await _documentCategoryService.CreateDocumentCategory(DocumentCategoryDTO);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: null,
                    message: "Document Category created successfully."
                ));
            }
            catch (Exception ex)
            {
                // Optional: Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status500InternalServerError,
                    code: ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    data: null,
                    message: "An unexpected error occurred."
                ));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocumentCategoryAsync(Guid id, [FromBody] UpdateDocumentCategoryDTO DocumentCategoryDTO)
        {
            if (DocumentCategoryDTO == null)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.BADREQUEST,
                data: null,
                    message: "Invalid Document Category data!"
                ));
            }

            try
            {
                await _documentCategoryService.UpdateDocumentCategory(id, DocumentCategoryDTO);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: null,
                    message: "Document Category updated successfully."
                ));
            }
            catch (Exception ex)
            {
                // Optional: Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status500InternalServerError,
                    code: ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    data: null,
                    message: "An unexpected error occurred."
                ));
            }
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteDocumentCategoryAsync(Guid id)
        {
            try
            {
                await _documentCategoryService.SoftDeleteDocumentCategory(id);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: null,
                    message: "Document Category deleted successfully."
                ));
            }
            catch (Exception ex)
            {
                // Optional: Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status500InternalServerError,
                    code: ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    data: null,
                    message: "An unexpected error occurred."
                ));
            }
        }

    }
}
