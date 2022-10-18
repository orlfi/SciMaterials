using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace SciMaterials.AUTH.Services;

/// <summary>
/// Утилиты по работе с jwt токенами
/// </summary>
public class AuthUtils : IAuthUtils
{
    private readonly IConfiguration _Configuration;
    private string _SecretKey;

    public AuthUtils(IConfiguration configuration)
    {
        _Configuration = configuration;
    }
    
    /// <summary>
    /// Метод создает jwt токен сессии
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="roles">Список ролей в системе</param>
    /// <returns>Возращает токен</returns>
    public string CreateSessionToken(IdentityUser user, IList<string> roles)
    {
        var config = _Configuration.GetSection("AuthApiSettings:SecretTokenKey");
        _SecretKey = config["key"];
        
        var jwt_security_token_handler = new JwtSecurityTokenHandler();
        
        var key = Encoding.ASCII.GetBytes(_SecretKey);

        //Claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var security_token_descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.ToArray()),
                
            Expires = DateTime.Now.AddMinutes(15),

            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
            
        var security_token = jwt_security_token_handler.CreateToken(security_token_descriptor);

        return jwt_security_token_handler.WriteToken(security_token);
    }
}