﻿using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.LawDocumentDTOs;
using DataAccess.ExceptionCustom;
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
            string? categoryNameSearch = null, string? filePathSearch = null, string? linkPathSearch = null, bool? expertVerificationSearch = null, [FromQuery(Name = "tagIds")] string[]? tagIdSearch = null)
        {
            PaginatedList<GetLawDocumentDTO> result = await _lawDocumentService.GetPaginatedLawDocumentsAsync(pageIndex, pageSize, idSearch, titleSearch, documentCodeSearch,
                categoryNameSearch, filePathSearch, linkPathSearch, expertVerificationSearch, tagIdSearch);
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
            catch (ErrorException ex)
            {
                // Return meaningful error to client (e.g., duplicate code)
                return StatusCode(ex.StatusCode, new BaseResponseModel<string>(
                    statusCode: ex.StatusCode,
                    code: ResponseCodeConstants.DUPLICATE,
                    data: null,
                    message: ex.Message
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
            catch (ErrorException ex)
            {
                // Return meaningful error to client (e.g., duplicate code)
                return StatusCode(ex.StatusCode, new BaseResponseModel<string>(
                    statusCode: ex.StatusCode,
                    code: ResponseCodeConstants.DUPLICATE,
                    data: null,
                    message: ex.Message
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

        /// <summary>
        /// Verify a law document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.Expert)]
        [HttpPost("verification/{id}")]
        public async Task<IActionResult> VerifyDocumentAsync(Guid id)
        {
            await _lawDocumentService.VerifyDocument(id);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Law Document verified successfully."
            ));
        }

        [HttpPost("upload")]
        //[Authorize(Roles = RoleConstants.Staff)]
        public async Task<IActionResult> UploadAndCreate(
        [FromForm] CreateLawDocumentRequest request)
        {
            try
            {
                // map CreateLawDocumentRequest → AddLawDocumentDTO
                var dto = new AddLawDocumentDTO
                {
                    Title = request.Title,
                    DocumentCode = request.DocumentCode,
                    CategoryId = request.CategoryId,
                    ExpertVerification = false,
                    TagList = request.TagList
                };

                // call your service
                await _lawDocumentService.CreateLawDocumentWithUploadAsync(dto, request.File);

                return StatusCode(StatusCodes.Status201Created,
                    new BaseResponseModel<string>(
                        StatusCodes.Status201Created,
                        ResponseCodeConstants.SUCCESS,
                        null,
                        "Law document uploaded and created successfully."
                    ));
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode,
                    new BaseResponseModel<string>(
                        ex.StatusCode,
                        ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                        null,
                        ex.Message
                    ));
            }
        }

    }
    }
