using Domain.Interfaces.Proxies;
using Microsoft.Extensions.Logging;

namespace Proxies
{
    public class ArticleApiProxy : IArticleApiProxy
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ArticleApiProxy> _logger;

        public ArticleApiProxy(HttpClient httpClient, ILogger<ArticleApiProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetAsync()
        {
            HttpResponseMessage responseMessage = await _httpClient.GetAsync("weatherforecast/name");

            string responseContent = await responseMessage.Content.ReadAsStringAsync();

            return responseContent + "dönen sonuç";
        }
    }
}
