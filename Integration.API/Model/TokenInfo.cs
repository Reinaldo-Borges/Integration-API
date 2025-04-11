using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Integration.API.Model
{
    public class TokenInfo
	{
		public Guid? UserId { get; }
		public string? Email { get; }
		public string? Role { get; }

		private readonly IEnumerable<Claim> _claims;

        public TokenInfo(JwtSecurityToken token)
		{
			_claims = token.Claims;

			UserId = HasValue("sub") ? Guid.Parse(GetClaimValue("sub")) : Guid.Empty;
			Email = HasValue("email") ? GetClaimValue("email") : string.Empty;
			Role = HasValue("role") ? GetClaimValue("role") : string.Empty;
        }

		public bool HasValue(string key)
		{
			var value = _claims.Where(c => c.Type == key).Select(s => s.Value);
			return value.Any();
        }

		public string GetClaimValue(string key)
		{
			return _claims.First(c => c.Type == key).Value;
        }
	}
}

