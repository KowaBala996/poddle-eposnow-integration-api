using poddle.Models;

namespace poddle.Services
{
    public interface IEposNowService
    {
        string GetAuthorizationUrl(string scopes = "sales products customers");
        Task<TokenResponse?> AuthorizeAsync(string authorizationCode);
        Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
        Task<List<Product>> GetProductsAsync(string accessToken);
        Task<List<Customer>> GetCustomersAsync(string accessToken);
        Task<List<Sale>> GetSalesAsync(string accessToken);
        Task<Product?> GetProductByIdAsync(int id, string accessToken);
        Task<Customer?> GetCustomerByIdAsync(int id, string accessToken);
    }
}