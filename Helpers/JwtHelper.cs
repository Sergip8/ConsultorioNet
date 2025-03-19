using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventManagementSystem.Helpers
{
    public static class JwtHelper
    {
        /// <summary>
        /// Generates a JWT token for the specified user ID and role.
        /// </summary>
        /// <param name="jwtSettings">The settings for configuring the JWT token generation, including secret key, issuer, and audience.</param>
        /// <param name="id">The unique identifier of the user for whom the token is generated.</param>
        /// <param name="role">The role of the user, used for authorization purposes.</param>
        /// <returns>A string representation of the generated JWT token.</returns>
        /// 
        
        public static string GenerateJwt(this JwtSettings jwtSettings, long id, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, id.ToString()),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.TokenExpirationInMinutes),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Decodes and validates the given JWT token string, returning the associated user ID and role.
        /// </summary>
        /// <param name="cookie">The JWT token string to be validated and decoded.</param>
        /// <param name="jwtSettings">The settings for validating the JWT token, including the secret key, issuer, and audience.</param>
        /// <returns>A <see cref="JwtData"/> object containing the user ID and role from the token.</returns>
        public static JwtData GetJwt(this JwtSettings jwtSettings, string cookie)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            // Create validation parameters with the secret key
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true
            };

            // Validate and decode the token
            var principal = tokenHandler.ValidateToken(cookie, validationParameters, out SecurityToken validatedToken);
            var id = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return new JwtData() { id = id, role = role };
        }

        /// <summary>
        /// Configures the options for setting a cookie that contains the JWT token.
        /// </summary>
        /// <param name="jwtSettings">The settings that define the cookie options, such as expiration time, secure flag, and SameSite mode.</param>
        /// <returns>A <see cref="CookieOptions"/> object configured for JWT token cookies.</returns>
        public static CookieOptions GetCookieOption(this JwtSettings jwtSettings)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.TokenExpirationInMinutes),
                SameSite = SameSiteMode.Strict
            };

            return cookieOptions;
        }
    }
}
