using poddle.Models;

namespace poddle.Repositories
{
    public interface IEposNowRepository
    {
        Task<TokenResponse?> GetAccessTokenAsync(string authorizationCode);
        Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
        Task<List<Product>> GetProductsAsync(string accessToken);
        Task<List<Customer>> GetCustomersAsync(string accessToken);
        Task<List<Sale>> GetSalesAsync(string accessToken);
        Task<Product?> GetProductByIdAsync(int id, string accessToken);
        Task<Customer?> GetCustomerByIdAsync(int id, string accessToken);
    }
}