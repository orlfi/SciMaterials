using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SciMaterials.Contracts.Auth;

namespace SciMaterials.AUTH.Services;

public interface IAuthUtils : IAuthUtils<IdentityUser>
{ }

/// <summary>
/// Утилиты по работе с jwt токенами
/// </summary>
public class AuthUtils : IAuthUtils
{
    private readonly IConfiguration _configuration;
    private string _secretKey;

    public AuthUtils(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Метод создает jwt токен сессии
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="roles">Список ролей в системе</param>
    /// <returns>Возращает токен</returns>
    public string CreateSessionToken(IdentityUser user, IList<string> roles)
    {
        var config = _configuration.GetSection("AuthApiSettings:SecretTokenKey");
        _secretKey = config["key"];
        
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        
        byte[] key = Encoding.ASCII.GetBytes(_secretKey);

        //Claims
        var claims = new List<Claim>();

        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        claims.Add(new Claim(ClaimTypes.Name, user.Email));
        claims.Add(new Claim(ClaimTypes.Email, user.Email));
        
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var securityTokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims.ToArray()),
                
            Expires = DateTime.Now.AddMinutes(15),

            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
            
        SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);

        return jwtSecurityTokenHandler.WriteToken(securityToken);
    }
}