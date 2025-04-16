using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Integration.API.Model;
using Integration.API.Model.ViewModel;
using Integration.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Integration.API.Services
{
    public class UserTokenHandler
	{
        private readonly IStudentService _service;
        private readonly UserManager<IdentityUser> _userManager;


        public UserTokenHandler(IStudentService service, UserManager<IdentityUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        public async Task<UserViewModel> BuildUser(JwtSettings jwtSettings, string email, string token)
        {            
            var student = await _service.GetByEmail(email);
            var tokenInfo = JwtDecoder(token);
            return new UserViewModel
            {
                Id = tokenInfo.UserId.Value,
                StudentId = student?.Id ?? Guid.Empty,
                Name = student?.Name ?? string.Empty,
                Email = email,
                Cellphone = student?.Cellphone ?? string.Empty,
                Birthday = student?.Birthday ?? null,
                Country = student?.Country ?? string.Empty,
                TypeStudentId = student?.TypeStudent ?? (int)TypeStudentEnum.Basic,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(jwtSettings.ExpriresInHours)
            };
        }


        public async Task<ClaimsIdentity> GetClaims(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var role = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()));
            claims.Add(new Claim("role", role.FirstOrDefault()));

            return new ClaimsIdentity(claims);
        }

        public async Task<string> GetJwt(JwtSettings jwtSettings, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.ValidIn,
                Expires = DateTime.UtcNow.AddHours(jwtSettings.ExpriresInHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Subject = await GetClaims(email)
            });

            var encondedToken = tokenHandler.WriteToken(token);
            return encondedToken;
        }       

        public TokenInfo JwtDecoder(string token)
        {            
            var objectToken = new JwtSecurityToken(token);

            return new TokenInfo(objectToken);
        }
    }
}