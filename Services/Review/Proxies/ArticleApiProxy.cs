using Application.Exceptions;
using Domain.Interfaces.Proxies;
using Domain.Interfaces.Proxies.Responses;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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

        public async Task<GetArticlesItemResponse> GetByIdAsync(Guid id)
        {
            HttpResponseMessage responseMessage = await _httpClient.GetAsync($"api/v1/articles?$select=id&$filter=Id eq {id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonResponse = await responseMessage.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<List<GetArticlesItemResponse>>(jsonResponse);

                return result.FirstOrDefault();
            }

            string responseContent = await responseMessage.Content.ReadAsStringAsync();

            string message = $"Error occured on Proxy:{nameof(ArticleApiProxy)}, Method:GET Endpoint:api/v1/articles, with StatusCode:{responseMessage.StatusCode}";

            _logger.LogError(message);

            throw new ProxyException(message);
        }
    }
}
