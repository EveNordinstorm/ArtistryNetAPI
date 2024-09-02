using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ArtistryNetAPI.Utilities
{
    public static class JwtHelper
    {
        public static string GetUserIdFromToken(HttpContext httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Authorization Header: {authHeader}");

            if (authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    var jwtToken = tokenHandler.ReadJwtToken(token);

                    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                    Console.WriteLine($"User ID Claim: {userIdClaim?.Value}");
                    return userIdClaim?.Value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing token: {ex.Message}");
                    return null;
                }
            }
            return null;
        }
    }
}
