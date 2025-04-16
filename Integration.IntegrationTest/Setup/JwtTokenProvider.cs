using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Integration.IntegrationTest.Setup
{
    public class JwtTokenProvider
	{
        public static string Issuer { get; } = "IntegrationTestApp";
        public static SecurityKey SecurityKey { get; } =
            new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes("MINHASUPERULTRAMEGAPOWERPASSWORDMUITOSECRETAMEGABLASTER")
            );
        public static SigningCredentials SigningCredentials { get; } =
            new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        internal static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();
    }
}

