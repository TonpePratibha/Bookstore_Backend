using DataAccessLayer.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.JWT
{


    public class JwtHelper
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public JwtHelper(IConfiguration configuration)
        {
            _secretKey = configuration["JwtSettings:SecretKey"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
            _expiryMinutes = int.Parse(configuration["JwtSettings:ExpiryMinutes"]);
        }

        /*     public string GenerateToken(User user)
             {
                 var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                 var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                 var claims = new[]
                 {
                                 new Claim(ClaimTypes.Name, user.Email),
                               //  new Claim("UserId", user.Id.ToString()),
                                 new Claim(ClaimTypes.Role, "User"),
                                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                 new Claim("userId", user.Id.ToString())
                             };


                 var token = new JwtSecurityToken(
                     issuer: _issuer,
                     audience: _audience,
                     claims: claims,
                     expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                     signingCredentials: credentials
                 );

                 return new JwtSecurityTokenHandler().WriteToken(token);
             }

             */


        public string GenerateToken(string email, string role, int id)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", id.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        /*    public string GenerateResetToken(User user)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        */


        public string GenerateResetToken(int id, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim("userId", id.ToString()),
        new Claim("role", role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public int ExtractUserIdFromJwt(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new InvalidOperationException("User ID not found in token.");
            }

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            throw new InvalidOperationException("Invalid User ID format in token.");
        }
    }

}