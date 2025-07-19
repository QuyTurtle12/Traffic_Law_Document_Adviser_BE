using AutoMapper;
using BusinessLogic.Helpers;
using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.AuthDTOs;
using DataAccess.Entities;
using DataAccess.ExceptionCustom;
using DataAccess.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLogic.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUOW _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
          IUOW unitOfWork,
          IMapper mapper,
          IOptions<JwtSettings> jwtConfig,
          IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtSettings = jwtConfig.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO dto)
        {
            // Check if email already exists
            var userRepo = _unitOfWork.GetRepository<User>();
            bool emailExists = userRepo.Entities.Any(u => u.Email == dto.Email);
            if (emailExists)
                throw new ErrorException(
                  StatusCodes.Status400BadRequest,
                  "EMAIL_EXISTS",
                  "Email is already registered."
                );

            // Create new User entity
            var newUser = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = RoleConstants.UserValue,
                IsActive = true
            };

            // Insert into database
            await userRepo.InsertAsync(newUser);
            await _unitOfWork.SaveAsync();

            // Generate JWT
            var tokenString = GenerateJwtToken(newUser);

            return new AuthResponseDTO
            {
                Token = tokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                UserId = newUser.Id.ToString(),
                Role = RoleConstants.User,
                FullName = newUser.FullName!,
                Email = newUser.Email!
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            // Look up user by email
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = userRepo.Entities.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
                throw new ErrorException(
                  StatusCodes.Status401Unauthorized,
                  "INVALID_CREDENTIALS",
                  "Email or password is incorrect."
                );

            // Verify password
            bool validPassword = PasswordHasher.Verify(dto.Password, user.PasswordHash!);
            if (!validPassword)
                throw new ErrorException(
                  StatusCodes.Status401Unauthorized,
                  "INVALID_CREDENTIALS",
                  "Email or password is incorrect."
                );

            // Check if user is active
            if (!user.IsActive)
                throw new ErrorException(
                  StatusCodes.Status403Forbidden,
                  "USER_INACTIVE",
                  "Your account has been disabled."
                );

            // Generate JWT
            var tokenString = GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                Token = tokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                UserId = user.Id.ToString(),
                Role = RoleConstants.ToRoleName(user.Role!.Value),
                FullName = user.FullName!,
                Email = user.Email!
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims: subject = user.Id, email, role
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Role,                RoleConstants.ToRoleName(user.Role!.Value)),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<User?> GetCurrentLoggedInUser()
        {
            // Get the current user's ID from the JWT claims
            string? currentId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            // If no ID is found, return null
            if (string.IsNullOrWhiteSpace(currentId)) return Task.FromResult<User?>(null);

            // Fetch the user from the database by ID
            return _unitOfWork.GetRepository<User>().GetByIdAsync(Guid.Parse(currentId));
        }
    }
}
