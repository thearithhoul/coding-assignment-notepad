using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using notepad_backend.Config;
using notepad_backend.Entities;


namespace notepad_backend.Func;


public class JwtFunc(IOptions<JwtSettings> jwtsetting)
{

    public string GenerateToken(bool isRefrash, AppUserEntity appUserEntity)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsetting.Value.SecretKey));

        var expriesat = DateTime.UtcNow.AddMinutes(30);
        if (isRefrash)
        {
            expriesat = DateTime.UtcNow.AddHours(1);
        }

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtsetting.Value.Issuer,
            audience: jwtsetting.Value.Audience,
            claims: new[] {
                new Claim(ClaimTypes.Name, appUserEntity.Username),
                new Claim("user_id", appUserEntity.Id.ToString()),
            },
            expires: expriesat,
            signingCredentials: creds
        );
        string jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

}