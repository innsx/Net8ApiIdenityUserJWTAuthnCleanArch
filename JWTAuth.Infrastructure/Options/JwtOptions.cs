namespace JWTAuth.Infrastructure.Options
{
    public class JwtOptions
    {
        public const string JwtOptionsKey = "JwtOptions";

        //NOTE: these values SHOULD BE STORED in Azure Key Vault
        // & then we RETREIVE their VALUES from AKV
        //For this App Demo, These are configured in AppSettings.Json file
        public string? Secret { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? ExpirationTimeInMinutes { get; set; }
    }
}
