using System.Security.Claims;
using Integration.API.Model;
using Integration.API.Model.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace Integration.API.Services
{
    public interface IUserHandlerService
	{
        Task<UserViewModel> BuildUser(IStudentService service, JwtSettings jwtSettings, string email, string token);
        Task<string> GetJwt(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, string email);
        Task<ClaimsIdentity> GetClaims(UserManager<IdentityUser> userManager, string email);
        string GeneratePassword();
    }
}

