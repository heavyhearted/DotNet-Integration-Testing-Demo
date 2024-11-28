using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Controllers;

public class IdentityServiceResponse
{
    public required string AccessToken { get; set; }
}

[ApiController]
public class IdentityController : ControllerBase
{
    /* This class is a demonstration helper for generating JSON Web Tokens (JWTs).
     It retrieves the TokenSecret from the FakeVault for demo purposes.
     In a real-world scenario, secrets like the TokenSecret must be stored securely,
     e.g., using environment variables, a secrets manager, or a vault.
     The TokenSecret used here is a symmetric key, which implies that the same key is utilized for both signing and verifying the token.
     This approach is not appropriate for production scenarios due to inherent security limitations.
     For improved security, consider using asymmetric keys (i.e., a public/private key pair),
     which provide a higher level of security by enabling separate keys for signing and verification processes.  */
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

    [HttpPost("token")]
    public IActionResult GenerateToken(
        [FromBody]TokenGenerationRequest request)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(FakeVault.Core.FakeVault.TokenSecret);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
            new("userid", request.UserId.ToString())
        };
        
        foreach (var claimPair in request.CustomClaims)
        {
            var jsonElement = (JsonElement)claimPair.Value;
            var valueType = jsonElement.ValueKind switch
            {
                JsonValueKind.True => ClaimValueTypes.Boolean,
                JsonValueKind.False => ClaimValueTypes.Boolean,
                JsonValueKind.Number => ClaimValueTypes.Double,
                _ => ClaimValueTypes.String
            };
            
            var claim = new Claim(claimPair.Key, claimPair.Value.ToString()!, valueType);
            claims.Add(claim);
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            Issuer = "https://id.dessy-snowboardshop.com",
            Audience = "https://api.snowboardshop.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);
        
        var response = new IdentityServiceResponse
        {
            AccessToken = jwt
        };
        
        return Ok(response);
    }
}
