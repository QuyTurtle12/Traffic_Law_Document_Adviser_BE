using BusinessLogic.IServices;
using DataAccess.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Mvc;

namespace Traffic_Law_Document_Adviser_API.Controllers
{

  [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(new
            {
                token = result.Token,
                expires = result.ExpiresAt,
                user = new
                {
                    result.UserId,
                    result.FullName,
                    result.Email,
                    result.Role
                }
            });
        }

        /// <summary>
        /// Login and receive JWT
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(new
            {
                token = result.Token,
                expires = result.ExpiresAt,
                user = new
                {
                    result.UserId,
                    result.FullName,
                    result.Email,
                    result.Role
                }
            });
        }
    }
}
