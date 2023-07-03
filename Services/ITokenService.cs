using System.Collections.Generic;
using System.Security.Claims;

namespace altenar_test_webapi.Services;
public interface ITokenService
{
    string GenerateToken(IEnumerable<Claim> claims);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}