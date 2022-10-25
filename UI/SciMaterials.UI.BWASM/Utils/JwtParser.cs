using System.Security.Claims;
using System.Text.Json;

namespace SciMaterials.UI.BWASM.Utils;

public static class JwtParser
{
    public static IReadOnlyCollection<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);

        var jsonClaims = JsonSerializer.Deserialize<List<ClaimJson>>(jsonBytes);
        if (jsonClaims is null) return Array.Empty<Claim>();

        ExtractRolesFromJwt(claims, jsonClaims);

        claims.AddRange(jsonClaims.Select(x => new Claim(x.Type, x.Value)));

        return claims;
    }

    private static void ExtractRolesFromJwt(List<Claim> claims, List<ClaimJson> jsonClaims)
    {
        var roles = jsonClaims.Where(x=>x.Type ==ClaimTypes.Role).ToList();
        
        claims.AddRange(roles.Select(x => new Claim(x.Type, x.Value)));

        foreach (ClaimJson claimJson in roles)
        {
            jsonClaims.Remove(claimJson);
        }
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

    private struct ClaimJson
    {
        public string Type { get; init; }
        public string Value { get; init; }
    }
}