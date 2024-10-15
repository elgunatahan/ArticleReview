using AuthApi.Dtos;
using AuthApi.Exceptions;
using AuthApi.Models.Requests;
using AuthApi.Models.Responses;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();
                throw new ValidationException(errors);
            }

            var token = await _userService.LoginAsync(new LoginInputDto
            {
                Password = loginRequest.Password,
                Username = loginRequest.Username
            });

            return Ok(new LoginResponse { Token = token });
        }
    }
}
