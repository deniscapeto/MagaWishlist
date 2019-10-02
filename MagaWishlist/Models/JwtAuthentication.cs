namespace MagaWishlist.Models
{
    public class JwtAuthentication
    {
        public string SecurityKey { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string ExpirationInMinutes { get; set; }
    }
}
