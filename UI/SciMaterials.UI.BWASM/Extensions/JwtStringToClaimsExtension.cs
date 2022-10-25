using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// ReSharper disable once CheckNamespace
namespace System;

public static class JwtStringToClaimsExtension
{
    public static IReadOnlyCollection<Claim> ParseJwt(this string jwt)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(jwt).Claims.ToList();
    }
}