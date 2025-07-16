using Microsoft.AspNetCore.Mvc;
using poddle.Services;

namespace poddle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EposNowController : ControllerBase
    {
        private readonly IEposNowService _eposNowService;
        private readonly ILogger<EposNowController> _logger;

        public EposNowController(IEposNowService eposNowService, ILogger<EposNowController> logger)
        {
            _eposNowService = eposNowService;
            _logger = logger;
        }

        [HttpGet("auth-url")]
        public IActionResult GetAuthorizationUrl([FromQuery] string scopes = "sales products customers")
        {
            var authUrl = _eposNowService.GetAuthorizationUrl(scopes);
            return Ok(new { authUrl });
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> Authorize([FromBody] AuthorizeRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
                return BadRequest("Authorization code is required");

            var tokenResponse = await _eposNowService.AuthorizeAsync(request.Code);
            if (tokenResponse == null)
                return BadRequest("Failed to get access token");

            return Ok(tokenResponse);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Refresh token is required");

            var tokenResponse = await _eposNowService.RefreshTokenAsync(request.RefreshToken);
            if (tokenResponse == null)
                return BadRequest("Failed to refresh token");

            return Ok(tokenResponse);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromHeader] string authorization)
        {
            var accessToken = ExtractBearerToken(authorization);
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized("Bearer token required");

            var products = await _eposNowService.GetProductsAsync(accessToken);
            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProduct(int id, [FromHeader] string authorization)
        {
            var accessToken = ExtractBearerToken(authorization);
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized("Bearer token required");

            var product = await _eposNowService.GetProductByIdAsync(id, accessToken);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers([FromHeader] string authorization)
        {
            var accessToken = ExtractBearerToken(authorization);
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized("Bearer token required");

            var customers = await _eposNowService.GetCustomersAsync(accessToken);
            return Ok(customers);
        }

        [HttpGet("customers/{id}")]
        public async Task<IActionResult> GetCustomer(int id, [FromHeader] string authorization)
        {
            var accessToken = ExtractBearerToken(authorization);
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized("Bearer token required");

            var customer = await _eposNowService.GetCustomerByIdAsync(id, accessToken);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSales([FromHeader] string authorization)
        {
            var accessToken = ExtractBearerToken(authorization);
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized("Bearer token required");

            var sales = await _eposNowService.GetSalesAsync(accessToken);
            return Ok(sales);
        }

        private string? ExtractBearerToken(string? authorization)
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                return null;

            return authorization.Substring("Bearer ".Length);
        }
    }

    public class AuthorizeRequest
    {
        public string Code { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}