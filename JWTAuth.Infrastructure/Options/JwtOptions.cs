using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuth.Infrastructure.Options
{
    public class JwtOptions
    {
        public const string JwtOptionsKey = "JwtOptions";

        public string? Secret { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? ExpirationTimeInMinutes { get; set; }
    }
}
