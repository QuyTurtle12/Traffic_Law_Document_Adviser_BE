using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.DocumentTagDTOs;
using DataAccess.PaginatedList;
using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{
    [Route("api/document-tags")]
    [ApiController]
    public class DocumentTagsController : ControllerBase
    {
        private readonly IDocumentTagService _documentTagService;

        // Costructor
        public DocumentTagsController(IDocumentTagService documentTagService)
        {
            _documentTagService = documentTagService;
        }

        /// <summary>
        /// Get paginated list of document tag
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="idSearch">DocumentTag id</param>
        /// <param name="nameSearch">DocumentTag name</param>
        /// <param name="parentNameSearch">DocumentTag parent name</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPaginatedListAsync(int pageIndex = 1, int pageSize = 10, string? idSearch = null, string? nameSearch = null, string? parentNameSearch = null)
        {
            PaginatedList<GetDocumentTagDTO>? result = await _documentTagService.GetPaginatedListAsync(pageIndex, pageSize, idSearch, nameSearch, parentNameSearch);

            return Ok(new BaseResponseModel<PaginatedList<GetDocumentTagDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Document tags retrieved successfully."
            ));
        }

        /// <summary>
        /// Get a document tag by id
        /// </summary>
        /// <param name="id">DocumentTag id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            GetDocumentTagDTO? result = await _documentTagService.GetByIdAsync(id);

            return Ok(new BaseResponseModel<GetDocumentTagDTO>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: result,
                    message: "Document tag retrieved successfully."
            ));
        }

        /// <summary>
        /// Create a new tag
        /// </summary>
        /// <param name="documentTagDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync(AddDocumentTagDTO documentTagDTO)
        {
            await _documentTagService.AddAsync(documentTagDTO);

            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status201Created,
                code: ResponseCodeConstants.SUCCESS,
                message: "Create a new tag successfully."
            ));
        }

        /// <summary>
        /// Update a tag
        /// </summary>
        /// <param name="id">DocumentTag id</param>
        /// <param name="documentTagDTO"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateDocumentTagDTO documentTagDTO)
        {
            await _documentTagService.UpdateAsync(id, documentTagDTO);

            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Update tag successfully."
            ));
        }

        /// <summary>
        /// Delete a tag
        /// </summary>
        /// <param name="id">DocumentTag id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _documentTagService.DeleteAsync(id);

            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Delete tag successfully."
            ));
        }
    }
}
