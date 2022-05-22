using System;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ASPNetCoreAPI.Core;
using ASPNetCoreAPI.Entities;

namespace ASPNetCoreAPI.Authorization
{
    public interface IJwtUtils
    {
        public string GenerateAccessToken(UserEntity user);
        public string GenerateRefreshToken(UserEntity user);
        public long? ValidateToken(string? token, string type);
    }
    public class JwtUtils: IJwtUtils
    {
        private readonly AppSettings _appSettings;

        public JwtUtils(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private string GenerateToken(UserEntity user, string type)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _appSettings.AccessTokenSecret;
            DateTime expires = type switch
            {
                "refresh" => DateTime.UtcNow.AddDays(30),
                "access" => DateTime.UtcNow.AddHours(3),
                _ => DateTime.UtcNow.AddHours(1)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {new Claim("id", user.Id.ToString())}),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public string GenerateAccessToken(UserEntity user)
        {
            return GenerateToken(user, "access");
        }

        public string GenerateRefreshToken(UserEntity user)
        {
            return GenerateToken(user, "refresh");
        }

        public long? ValidateToken(string? token, string type)
        {
            if (token == null)
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            string secret = type switch
            {
                "access" => _appSettings.AccessTokenSecret,
                "refresh" => _appSettings.RefreshTokenSecret,
                _ => ""
            };

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = (JwtSecurityToken) validatedToken;
                var userId = long.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}