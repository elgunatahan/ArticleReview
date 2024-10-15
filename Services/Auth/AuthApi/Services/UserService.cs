using AuthApi.Dtos;
using AuthApi.Exceptions;
using AuthApi.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthApi.Services
{
    public interface IUserService
    {
        public Task<string> LoginAsync(LoginInputDto input);
    }
    public class UserService : IUserService
    {
        private readonly string _issuer;
        private readonly RsaSecurityKey _privateKey;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, RsaSecurityKey privateKey)
        {
            _issuer = Environment.GetEnvironmentVariable("JwtIssuer");
            _userRepository = userRepository;
            _privateKey = privateKey;
        }

        public async Task<string> LoginAsync(LoginInputDto input)
        {
            var user = await _userRepository.GetUserByUsernameAsync(input.Username);

            if (user == null || VerifyPassword(input.Password, user.PasswordHash) == false)
            {
                throw new AuthenticationFailedException();
            }

            var token = GenerateJwtToken(user.Username, user.Role);

            return token;
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        private string GenerateJwtToken(string username, string role)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var creds = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
