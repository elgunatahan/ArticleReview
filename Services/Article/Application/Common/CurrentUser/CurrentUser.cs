using Microsoft.AspNetCore.Http;

namespace Application.Common.CurrentUser
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;
        public HttpContext HttpContext => _accessor.HttpContext;

        public CurrentUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Username => HttpContext.User.Claims.FirstOrDefault(x => x.Type.StartsWith("username")).Value;
    }
}