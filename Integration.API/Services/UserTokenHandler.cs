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
        public async Task<UserViewModel> BuildUser(IStudentService service, JwtSettings jwtSettings, string email, string token)
        {            
            var student = await service.GetByEmail(email);
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


        public async Task<ClaimsIdentity> GetClaims(UserManager<IdentityUser> userManager, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            var role = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()));
            claims.Add(new Claim("role", role.FirstOrDefault()));

            return new ClaimsIdentity(claims);
        }

        public async Task<string> GetJwt(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.ValidIn,
                Expires = DateTime.UtcNow.AddHours(jwtSettings.ExpriresInHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Subject = await GetClaims(userManager, email)
            });

            var encondedToken = tokenHandler.WriteToken(token);
            return encondedToken;
        }

        public string GeneratePassword()
        {
            var lowerCase = "abcdefghijklmnopqrstuvwxyz";
            var upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var simble = "@#$&";
            var number = "0123456789";
            var random = new Random();
            var resultLowerCase = new string(Enumerable.Repeat(lowerCase, 3).Select(s => s[random.Next(s.Length)]).ToArray());
            var resultUpperCase = new string(Enumerable.Repeat(upperCase, 3).Select(s => s[random.Next(s.Length)]).ToArray());
            var resulSimple = new string(Enumerable.Repeat(simble, 1).Select(s => s[random.Next(s.Length)]).ToArray());
            var resultNumber = new string(Enumerable.Repeat(number, 2).Select(s => s[random.Next(s.Length)]).ToArray());
            var list = new List<string>() { resultLowerCase, resulSimple, resultUpperCase, resultNumber };
            return string.Join("", list);
        }

        public TokenInfo JwtDecoder(string token)
        {            
            var objectToken = new JwtSecurityToken(token);

            return new TokenInfo(objectToken);
        }
    }
}