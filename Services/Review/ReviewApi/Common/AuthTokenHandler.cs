using Microsoft.Extensions.Primitives;

namespace ReviewApi.Common
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Contains("Authorization"))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authorizationHeader))
            {
                request.Headers.Add("Authorization", authorizationHeader.ToString());
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
