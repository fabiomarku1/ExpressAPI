using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Utility;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace ExpressAPI.Utility;

public interface IJwtUtils
{
    (int?, string, string) ValidateToken(string accessToken);
}

public class JwtUtils : IJwtUtils
{
    private readonly JwtConfiguration _jwtConfiguration;

    public JwtUtils(IOptions<JwtConfiguration> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration.Value;
    }

    public (int?, string, string) ValidateToken(string accessToken)
    {
        if (accessToken == null)
            return (null, "", "");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfiguration.SecretKey);
        try
        {
            tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);
            var userEmail = jwtToken.Claims.First(x => x.Type == "Email").Value;
            var tokenHash = jwtToken.Claims.First(x => x.Type == "TokenHash").Value;

            return (userId, userEmail, tokenHash);
        }
        catch
        {
            // return null and empty if validation fails
            return (null, "", "");
        }
    }
}

