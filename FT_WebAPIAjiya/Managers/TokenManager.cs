using FT_WebAPIAjiya.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FT_WebAPIAjiya.Managers
{
    public class TokenManager
    {
        //const string skey = "xddgrrr446wwsggrge";
        //internal const string iss = "leong";
        //internal const string aud = "gnoel";

        const string skey = "dGP3EosHrMs3VjxFmMF3gYb8cPYf9xp2E8vTHJimwo";
        internal const string iss = "ftzx";
        internal const string aud = "Ajiya";

        public const string auth_type = "Cookies";

        internal static string GenerateToken(User user, double seconds = 86400 /* 24 HOURS */)
        {
            var credentials = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = iss,
                Audience = aud,
                Subject = new ClaimsIdentity(claims, "bearer"),
                Expires = DateTime.Now.AddSeconds(seconds),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var ctoken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenstr = tokenHandler.WriteToken(ctoken);

            return tokenstr;
        }

        private static SymmetricSecurityKey GetSecurityKey() => new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(skey));
    }
}
