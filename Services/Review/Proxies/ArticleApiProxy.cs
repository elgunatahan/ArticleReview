using Domain.Interfaces.Proxies;
using Domain.Interfaces.Proxies.Responses;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
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
                //var result = await responseMessage.Content.ReadFromJsonAsync<GetArticlesResponse>();

                var jsonResponse = await responseMessage.Content.ReadAsStringAsync();

                // JSON içindeki 'value' alanını deserialize etme
                var result = JsonSerializer.Deserialize<List<GetArticlesItemResponse>>(jsonResponse);

                return result.FirstOrDefault();

                //return result;
            }

            string responseContent = await responseMessage.Content.ReadAsStringAsync();

            _logger.LogError(responseContent);

            throw new Exception(responseContent);
        }
    }
}
