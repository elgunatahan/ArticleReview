namespace Domain.Interfaces.Proxies
{
    public interface IArticleApiProxy
    {
        Task<string> GetAsync();
    }
}
