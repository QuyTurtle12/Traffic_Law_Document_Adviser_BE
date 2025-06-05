using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.AuthDTOs;
using DataAccess.ResponseModel;
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
            // ModelState is automatically validated by ASP.NET Core because of [ApiController].
            var result = await _authService.RegisterAsync(dto);

            var response = new BaseResponseModel<AuthResponseDTO>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "User registered successfully."
            );

            return Ok(response);
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

            var response = new BaseResponseModel<AuthResponseDTO>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Login successful."
            );

            return Ok(response);
        }
    }
}
