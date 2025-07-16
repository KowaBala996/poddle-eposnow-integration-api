namespace poddle.Models
{
    public class EposNowConfig
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string PackageKey { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.eposnowhq.com/api/V2";
        public string AuthUrl { get; set; } = "https://auth.eposnowhq.com";
    }
}