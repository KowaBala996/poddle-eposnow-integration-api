using Microsoft.Extensions.Options;
using poddle.Models;
using poddle.Repositories;

namespace poddle.Services
{
    public class EposNowService : IEposNowService
    {
        private readonly IEposNowRepository _repository;
        private readonly EposNowConfig _config;
        private readonly ILogger<EposNowService> _logger;

        public EposNowService(IEposNowRepository repository, IOptions<EposNowConfig> config, ILogger<EposNowService> logger)
        {
            _repository = repository;
            _config = config.Value;
            _logger = logger;
        }

        public string GetAuthorizationUrl(string scopes = "sales products customers")
        {
            var authUrl = $"{_config.AuthUrl}/authorize" +
                         $"?response_type=code" +
                         $"&client_id={_config.ClientId}" +
                         $"&redirect_uri={Uri.EscapeDataString(_config.RedirectUri)}" +
                         $"&scope={Uri.EscapeDataString(scopes)}";
            
            return authUrl;
        }

        public async Task<TokenResponse?> AuthorizeAsync(string authorizationCode)
        {
            _logger.LogInformation("Authorizing with code: {Code}", authorizationCode);
            return await _repository.GetAccessTokenAsync(authorizationCode);
        }

        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Refreshing token");
            return await _repository.RefreshTokenAsync(refreshToken);
        }

        public async Task<List<Product>> GetProductsAsync(string accessToken)
        {
            _logger.LogInformation("Fetching products");
            return await _repository.GetProductsAsync(accessToken);
        }

        public async Task<List<Customer>> GetCustomersAsync(string accessToken)
        {
            _logger.LogInformation("Fetching customers");
            return await _repository.GetCustomersAsync(accessToken);
        }

        public async Task<List<Sale>> GetSalesAsync(string accessToken)
        {
            _logger.LogInformation("Fetching sales");
            return await _repository.GetSalesAsync(accessToken);
        }

        public async Task<Product?> GetProductByIdAsync(int id, string accessToken)
        {
            _logger.LogInformation("Fetching product with ID: {Id}", id);
            return await _repository.GetProductByIdAsync(id, accessToken);
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id, string accessToken)
        {
            _logger.LogInformation("Fetching customer with ID: {Id}", id);
            return await _repository.GetCustomerByIdAsync(id, accessToken);
        }
    }
}