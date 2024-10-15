using Domain.Interfaces.Proxies.Responses;

namespace Domain.Interfaces.Proxies
{
    public interface IArticleApiProxy
    {
        Task<GetArticleByIdProxyResponse> GetByIdAsync(Guid id);
    }
}
