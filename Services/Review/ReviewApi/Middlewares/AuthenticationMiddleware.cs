using System.Security.Claims;

namespace ReviewApi.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthenticationMiddleware(
            RequestDelegate next,
            IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            List<Claim> claims = new List<Claim>
                {
                    new Claim("username", "elgun")
                };

            ClaimsIdentity appIdentity = new ClaimsIdentity(claims);

            httpContext.User.AddIdentity(appIdentity);

            await _next(httpContext);
        }
    }
}
