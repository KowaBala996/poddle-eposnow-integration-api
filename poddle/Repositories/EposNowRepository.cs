using Microsoft.Extensions.Options;
using poddle.Models;
using System.Text;
using System.Text.Json;

namespace poddle.Repositories
{
    public class EposNowRepository : IEposNowRepository
    {
        private readonly HttpClient _httpClient;
        private readonly EposNowConfig _config;
        private readonly ILogger<EposNowRepository> _logger;

        public EposNowRepository(HttpClient httpClient, IOptions<EposNowConfig> config, ILogger<EposNowRepository> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _logger = logger;
        }
        public async Task<TokenResponse?> GetAccessTokenAsync(string authorizationCode)
        {
            var tokenUrl = $"{_config.AuthUrl}/token";
            var parameters = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", authorizationCode},
                {"redirect_uri", _config.RedirectUri},
                {"client_id", _config.ClientId},
                {"client_secret", _config.ClientSecret}
            };

            var content = new FormUrlEncodedContent(parameters);
            
            try
            {
                var response = await _httpClient.PostAsync(tokenUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TokenResponse>(json);
                }
                _logger.LogError("Failed to get access token: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting access token");
                return null;
            }
        }

        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            var tokenUrl = $"{_config.AuthUrl}/token";
            var parameters = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken},
                {"client_id", _config.ClientId},
                {"client_secret", _config.ClientSecret}
            };

            var content = new FormUrlEncodedContent(parameters);
            
            try
            {
                var response = await _httpClient.PostAsync(tokenUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TokenResponse>(json);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return null;
            }
        }

        public async Task<List<Product>> GetProductsAsync(string accessToken)
        {
            return await GetApiDataAsync<List<Product>>("/Product", accessToken) ?? new List<Product>();
        }

        public async Task<List<Customer>> GetCustomersAsync(string accessToken)
        {
            return await GetApiDataAsync<List<Customer>>("/Customer", accessToken) ?? new List<Customer>();
        }

        public async Task<List<Sale>> GetSalesAsync(string accessToken)
        {
            return await GetApiDataAsync<List<Sale>>("/Sale", accessToken) ?? new List<Sale>();
        }

        public async Task<Product?> GetProductByIdAsync(int id, string accessToken)
        {
            return await GetApiDataAsync<Product>($"/Product/{id}", accessToken);
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id, string accessToken)
        {
            return await GetApiDataAsync<Customer>($"/Customer/{id}", accessToken);
        }

        private async Task<T?> GetApiDataAsync<T>(string endpoint, string accessToken)
        {
            var url = $"{_config.BaseUrl}{endpoint}";
            
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("PackageKey", _config.PackageKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                _logger.LogError("API call failed: {StatusCode} for {Endpoint}", response.StatusCode, endpoint);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling API endpoint: {Endpoint}", endpoint);
                return default;
            }
        }
    }
}