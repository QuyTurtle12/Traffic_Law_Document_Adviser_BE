using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.DocumentTagMapDTOs;
using DataAccess.DTOs.LawDocumentDTOs;
using DataAccess.Entities;
using DataAccess.ExceptionCustom;
using DataAccess.IRepositories;
using DataAccess.PaginatedList;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Services
{
    public class LawDocumentService : ILawDocumentService
    {
        private readonly IPdfService _photoService;
        private readonly IMapper _mapper;
        private readonly IUOW _unitOfWork;
        private readonly IAuthService _authService;

        // Constructor
        public LawDocumentService(
            IPdfService photoService,
            IMapper mapper,
            IUOW unitOfWork,
            IAuthService authService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task CreateLawDocumentWithUploadAsync(AddLawDocumentDTO dto, IFormFile file)
        {
            // 1) check for PDF
            if (file == null || file.Length == 0)
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST,
                    "A PDF file is required.");

            var ext = Path.GetExtension(file.FileName);
            if (!string.Equals(ext, ".pdf", StringComparison.OrdinalIgnoreCase))
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST,
                    "Only PDF files are allowed.");

            // 2) upload to Cloudinary
            var publicName = Path.GetFileNameWithoutExtension(file.FileName);
            var url = await _photoService.UploadImageAsync(file, publicName);

            if (url == null)
                throw new ErrorException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    "Failed to upload PDF to Cloudinary.");

            // 3) set linkPath & create law document
            dto.LinkPath = url;
            await CreateLawDocument(dto);
        }


        public async Task<PaginatedList<GetLawDocumentDTO>> GetPaginatedLawDocumentsAsync(int pageIndex, int pageSize, Guid? idSearch, string? titleSearch, string? documentCodeSearch,
            string? categoryNameSearch, string? filePathSearch, string? linkPathSearch, bool? expertVerificationSearch, string[]? tagIdSearch)
        {
            if (pageIndex < 1 && pageSize < 1)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Page index or page size must be greater than or equal to 1.");
            }

            IQueryable<LawDocument> query = _unitOfWork.GetRepository<LawDocument>().Entities
                .Where(dc => !dc.DeletedTime.HasValue)
                .Include(dc => dc.Category)
                .Include(dc => dc.DocumentTagMaps)!
                    .ThenInclude(dc => dc.Tag)
                .Include(dc => dc.Expert);
            

            // Apply id search filters if provided
            if (idSearch.HasValue)
            {
                query = query.Where(p => p.Id.Equals(idSearch.Value));
            }

            if (!titleSearch.IsNullOrEmpty())
            {
                query = query.Where(p => p.Title!.Contains(titleSearch!));
            }

            if (!documentCodeSearch.IsNullOrEmpty())
            {
                query = query.Where(p => p.DocumentCode!.Contains(documentCodeSearch!));
            }

            if (!categoryNameSearch.IsNullOrEmpty())
            {

                query = query.Where(p => p.Category!.Name!.Contains(categoryNameSearch!));
            }

            if (!filePathSearch.IsNullOrEmpty())
            {
                query = query.Where(p => p.FilePath!.Contains(filePathSearch!));
            }

            if (!linkPathSearch.IsNullOrEmpty())
            {
                query = query.Where(p => p.LinkPath!.Contains(linkPathSearch!));
            }

            if (expertVerificationSearch.HasValue)
            {
                query = query.Where(p => p.ExpertVerification == expertVerificationSearch);
            }

            // Apply tag search filter if provided
            if (tagIdSearch != null && tagIdSearch.Length > 0)
            {
                // Convert string array to Guid array
                var tagGuids = tagIdSearch.Select(t => Guid.Parse(t)).ToArray();

                // Filter documents that have ALL the specified tags
                query = query.Where(doc =>
                    tagGuids.All(tagId =>
                        doc.DocumentTagMaps!.Any(map =>
                            map.DocumentTagId == tagId)));
            }

            query = query.OrderByDescending(p => p.CreatedTime);

            // Change to paginated list to facilitate mapping process
            PaginatedList<LawDocument> resultQuery = await _unitOfWork.GetRepository<LawDocument>()
                .GetPagging(query, pageIndex, pageSize);

            // Map the result to GetLawDocumentDTO
            IReadOnlyCollection<GetLawDocumentDTO> result = resultQuery.Items.Select(item =>
            {
                GetLawDocumentDTO lawDocumentDTO = _mapper.Map<GetLawDocumentDTO>(item);

                lawDocumentDTO.TagList = item.DocumentTagMaps?.Select(tagMap => new GetDocumentTagMapDTO
                {
                    Id = tagMap.DocumentTagId,
                    TagName = tagMap.Tag?.Name ?? string.Empty
                }).ToList() ?? new List<GetDocumentTagMapDTO>();

                lawDocumentDTO.VerifyBy = item.Expert?.Email ?? string.Empty;

                return lawDocumentDTO;
            }).ToList();

            PaginatedList<GetLawDocumentDTO> paginatedList = new PaginatedList<GetLawDocumentDTO>(result, resultQuery.TotalCount, resultQuery.PageNumber, resultQuery.PageSize);

            return paginatedList;
        }

        public async Task<GetLawDocumentDTO> GetLawDocumentById(Guid id)
        {
            LawDocument? lawDocument = await _unitOfWork.GetRepository<LawDocument>().GetByIdAsync(id);
            if (lawDocument == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Law document not found!");
            }
            GetLawDocumentDTO responseItem = _mapper.Map<GetLawDocumentDTO>(lawDocument);
            return responseItem;
        }

        public async Task CreateLawDocument(AddLawDocumentDTO lawDocumentDTO)
        {
            if (lawDocumentDTO == null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Law document data is required!");
            }

            // Check for existing DocumentCode
            // This cant create in case. StrinG2 cant be created when string2 exists
            //bool isDocumentCodeExist = await _unitOfWork.GetRepository<LawDocument>()
            //    .Entities
            //    .AnyAsync(ld =>
            //        ld.DocumentCode.ToLower() == lawDocumentDTO.DocumentCode!.ToLower() &&
            //        !ld.DeletedTime.HasValue);

            // This can create in case. StrinG2 cant be created when string2 exists
            bool isDocumentCodeExist = await _unitOfWork.GetRepository<LawDocument>()
                .Entities
                .AnyAsync(ld =>
                    EF.Functions.Collate(ld.DocumentCode, "Latin1_General_CS_AS") == lawDocumentDTO.DocumentCode &&
                    !ld.DeletedTime.HasValue);

            if (isDocumentCodeExist)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, "A law document with the same DocumentCode already exists.");
            }

            LawDocument lawDocument = _mapper.Map<LawDocument>(lawDocumentDTO);
            lawDocument.CreatedBy = "System";
            lawDocument.CreatedTime = DateTime.Now;

            // Insert the LawDocument into the database
            await _unitOfWork.GetRepository<LawDocument>().InsertAsync(lawDocument);
            await _unitOfWork.SaveAsync();

            // Add DoumentTagMaps
            if (lawDocumentDTO.TagList != null && lawDocumentDTO.TagList.Any())
            {
                List<DocumentTagMap> documentTagMaps = lawDocumentDTO.TagList.Select(tagMap => new DocumentTagMap
                {
                    DocumentTagId = tagMap.DocumentTagId,
                    DocumentId = lawDocument.Id
                }).ToList();

                // Add each DocumentTagMap to the database
                foreach (DocumentTagMap tagMap in documentTagMaps)
                {
                    await _unitOfWork.GetRepository<DocumentTagMap>().InsertAsync(tagMap);
                }
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateLawDocument(Guid id, UpdateLawDocumentDTO lawDocumentDTO)
        {
            // Get document by id
            IGenericRepository<LawDocument> repository = _unitOfWork.GetRepository<LawDocument>();
            LawDocument? existingLawDocument = await repository.Entities
                .Where(dc => dc.Id == id)
                .Include(dc => dc.DocumentTagMaps)
                .FirstOrDefaultAsync();

            // Check if the law document exists
            if (existingLawDocument == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.BADREQUEST, "Law Document not found!");
            }

            // Check for existing DocumentCode
            bool isDocumentCodeExist = await _unitOfWork.GetRepository<LawDocument>()
                .Entities
                .AnyAsync(ld =>
                    ld.Id != id &&
                    EF.Functions.Collate(ld.DocumentCode, "Latin1_General_CS_AS") == lawDocumentDTO.DocumentCode &&
                    !ld.DeletedTime.HasValue);

            if (isDocumentCodeExist)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, "A law document with the same DocumentCode already exists.");
            }

            // Map the DTO to the existing LawDocument entity
            LawDocument lawDocument = _mapper.Map(lawDocumentDTO, existingLawDocument);
            lawDocument.LastUpdatedBy = "System";
            lawDocument.LastUpdatedTime = DateTime.Now;

            // Update the LawDocument in the database
            repository.Update(existingLawDocument);
            await _unitOfWork.SaveAsync();

            // Update DoumentTagMaps
            if (lawDocumentDTO.TagList != null && lawDocumentDTO.TagList.Any())
            {
                // Clear existing DocumentTagMaps
                if (existingLawDocument.DocumentTagMaps != null && existingLawDocument.DocumentTagMaps.Any())
                {
                    foreach (DocumentTagMap tagMap in existingLawDocument.DocumentTagMaps)
                    {
                        _unitOfWork.GetRepository<DocumentTagMap>().Delete(tagMap);
                    }
                }

                // Add new DocumentTagMaps
                List<DocumentTagMap> documentTagMaps = lawDocumentDTO.TagList.Select(tagMap => new DocumentTagMap
                {
                    DocumentTagId = tagMap.DocumentTagId,
                    DocumentId = lawDocument.Id
                }).ToList();

                // Add each DocumentTagMap to the database
                foreach (DocumentTagMap tagMap in documentTagMaps)
                {
                    await _unitOfWork.GetRepository<DocumentTagMap>().InsertAsync(tagMap);
                }
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteLawDocument(Guid id)
        {
            IGenericRepository<LawDocument> repository = _unitOfWork.GetRepository<LawDocument>();
            LawDocument? existingLawDocument = await repository.GetByIdAsync(id);
            if (existingLawDocument == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.BADREQUEST, "Law Document not found!");
            }

            repository.Delete(existingLawDocument);
            await _unitOfWork.SaveAsync();
        }

        public async Task SoftDeleteLawDocument(Guid id)
        {
            // Get document by id
            IGenericRepository<LawDocument> repository = _unitOfWork.GetRepository<LawDocument>();
            LawDocument? existingLawDocument = await repository.Entities
                .Where(dc => dc.Id == id)
                .Include(dc => dc.DocumentTagMaps)
                .FirstOrDefaultAsync();

            // Check if the law document exists
            if (existingLawDocument == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.BADREQUEST, "Law document not found!");
            }
            existingLawDocument.DeletedBy = "System";
            existingLawDocument.DeletedTime = DateTime.Now;
            existingLawDocument.LastUpdatedBy = "System";
            existingLawDocument.LastUpdatedTime = DateTime.Now;

            // Clear related DocumentTagMaps
            if (existingLawDocument.DocumentTagMaps != null && existingLawDocument.DocumentTagMaps.Any())
            {
                foreach (DocumentTagMap tagMap in existingLawDocument.DocumentTagMaps)
                {
                    _unitOfWork.GetRepository<DocumentTagMap>().Delete(tagMap);
                }
            }

            // Update the LawDocument in the database
            repository.Update(existingLawDocument);
            await _unitOfWork.SaveAsync();
        }

        public async Task VerifyDocument(Guid id)
        {
            // Get document by id
            LawDocument? lawDocument = await _unitOfWork.GetRepository<LawDocument>().Entities
                .Where(dc => dc.Id == id && !dc.DeletedTime.HasValue)
                .FirstOrDefaultAsync();

            // Check if the law document exists
            if (lawDocument == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Law document not found!");
            }

            // Verify the document
            lawDocument.ExpertVerification = true;

            User? currentUser = await _authService.GetCurrentLoggedInUser();

            // Check if the user is authenticated
            if (currentUser == null)
            {
                throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "User is not authenticated.");
            }

            // Set the user who verified the document
            lawDocument.VerifyBy = currentUser.Id;

            // Set the last updated information
            lawDocument.LastUpdatedBy = currentUser.Email;
            lawDocument.LastUpdatedTime = DateTime.Now;

            // Update the LawDocument in the database
            await _unitOfWork.GetRepository<LawDocument>().UpdateAsync(lawDocument);
            await _unitOfWork.SaveAsync();
        }
    }
}
