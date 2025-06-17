using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.LawDocumentDTOs;
using DataAccess.PaginatedList;
using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{
    [Route("api/law-documents")]
    [ApiController]
    public class LawDocumentController : Controller
    {
        private readonly ILawDocumentService _lawDocumentService;

        // Constructor
        public LawDocumentController(ILawDocumentService lawDocumentService)
        {
            _lawDocumentService = lawDocumentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedCartsAsync(int pageIndex = 1, int pageSize = 10, Guid? idSearch = null, string? titleSearch = null, string? documentCodeSearch = null,
            string? categoryNameSearch = null, string? filePathSearch = null, string? linkPathSearch = null, bool? expertVerificationSearch = null)
        {
            PaginatedList<GetLawDocumentDTO> result = await _lawDocumentService.GetPaginatedLawDocumentsAsync(pageIndex, pageSize, idSearch, titleSearch, documentCodeSearch, 
                categoryNameSearch, filePathSearch, linkPathSearch, expertVerificationSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetLawDocumentDTO>>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: result,
                    message: "Law documents retrieved successfully."
                ));
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Staff)]
        public async Task<IActionResult> PostLawDocumentAsync(AddLawDocumentDTO lawDocumentDTO)
        {
            if (lawDocumentDTO == null)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.BADREQUEST,
                    data: null,
                    message: "Law Document data is required!"
                ));
            }

            try
            {
                await _lawDocumentService.CreateLawDocument(lawDocumentDTO);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status201Created,
                    code: ResponseCodeConstants.SUCCESS,
                    data: null,
                    message: "Law Document created successfully."
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

        [Authorize(Roles = RoleConstants.Staff)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLawDocumentAsync(Guid id, [FromBody] UpdateLawDocumentDTO lawDocumentDTO)
        {
            if (lawDocumentDTO == null)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.BADREQUEST,
                    data: null,
                    message: "Invalid law document data!"
                ));
            }

            try
            {
                await _lawDocumentService.UpdateLawDocument(id, lawDocumentDTO);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: null,
                    message: "Law document updated successfully."
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

        [Authorize(Roles = $"{RoleConstants.Staff},{RoleConstants.Admin}")]
        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteLawDocumentAsync(Guid id)
        {
            try
            {
                await _lawDocumentService.SoftDeleteLawDocument(id);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: null,
                    message: "Law Document deleted successfully."
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
