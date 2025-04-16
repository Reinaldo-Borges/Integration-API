using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Integration.IntegrationTest.Builder;
using Integration.IntegrationTest.Setup;

namespace Integration.IntegrationTest
{
    public class BaseTest
	{
        private readonly APIAppFactory _apiAppFactory;
        public readonly HttpClient _client;
        public readonly string? _token;

        public BaseTest(APIAppFactory apiAppFactory)
        {
            _apiAppFactory = apiAppFactory;
            _client = _apiAppFactory.CreateClient();
            _token = JwtTokenProvider.JwtSecurityTokenHandler.WriteToken(
                                            new JwtSecurityToken(
                                                    JwtTokenProvider.Issuer,
                                                    JwtTokenProvider.Issuer,
                                                    new List<Claim> {
                                                        new(ClaimTypes.Role, "Premium"), },
                                                            expires: DateTime.Now.AddMinutes(30),
                                                            signingCredentials: JwtTokenProvider.SigningCredentials));

        }

        public async Task CreatedRoles()
        {
            var command = @"INSERT INTO dbo.AspNetRoles(Id, Name, NormalizedName)
                            VALUES(1 , 'Basic', 'BASIC');
                            INSERT INTO dbo.AspNetRoles(Id, Name, NormalizedName) 
                            VALUES(2 , 'Partner', 'PARTNER');
                            INSERT INTO dbo.AspNetRoles(Id, Name, NormalizedName)
                            VALUES(3 , 'Premium', 'PREMIUM');";

            await _apiAppFactory.ExecuteCommandAsync(command);
        }

        public async Task CreatedUser(UserBuilder userBuilder)
        {
            var commandNewUser = @"INSERT INTO AspNetUsers
                                    (Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,
                                        PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount)
                                    VALUES
                                    (@Id,@UserName,@NormalizedUserName,@Email,@NormalizedEmail,@EmailConfirmed,@PasswordHash,@SecurityStamp,
                                        @PhoneNumberConfirmed,@TwoFactorEnabled,@LockoutEnabled,@AccessFailedCount)";
            var param = new
            {
                Id = userBuilder.Id,
                UserName = userBuilder.UserName,
                NormalizedUserName = userBuilder.NormalizedUserName,
                Email = userBuilder.Email,
                NormalizedEmail = userBuilder.NormalizedEmail,
                EmailConfirmed = userBuilder.EmailConfirmed,
                PasswordHash = userBuilder.PasswordHash,
                SecurityStamp = userBuilder.SecurityStamp,
                PhoneNumberConfirmed = userBuilder.PhoneNumberConfirmed,
                TwoFactorEnabled = userBuilder.TwoFactorEnabled,
                LockoutEnabled = userBuilder.LockoutEnabled,
                AccessFailedCount = userBuilder.AccessFailedCount
            };

            await _apiAppFactory.ExecuteCommandAsync(commandNewUser, param);
        }

        public async Task CreatedUserRole(Guid userId, int roleId)
        {
            var command = "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
            await _apiAppFactory.ExecuteCommandAsync(command, new
            {
                UserId = userId,
                RoleId = roleId
            });
        }

        public async Task CreatedTypeStudent()
        {
            var command = @"INSERT INTO TypeStudent(Id, Name)
                            VALUES(1 , 'Basic');
                            INSERT INTO TypeStudent(Id, Name) 
                            VALUES(2 , 'Partner');
                            INSERT INTO TypeStudent(Id, Name)
                            VALUES(3 , 'Premium');";

            await _apiAppFactory.ExecuteCommandAsync(command);
        }

    }
}

