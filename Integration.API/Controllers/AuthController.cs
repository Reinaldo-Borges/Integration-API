using Integration.API.Model;
using Integration.API.Model.Request;
using Integration.API.Model.ViewModel;
using Integration.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Integration.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IStudentService _service;      
        private IUserHandlerService _userHandlerService;

        public AuthController(SignInManager<IdentityUser> signInManager,
                                            UserManager<IdentityUser> userManager,
                                            IOptions<JwtSettings> jwtSettings,
                                            IStudentService service,
                                            IUserHandlerService userHandlerService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _service = service;
            _userHandlerService = userHandlerService;
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RegisterUserViewModel), StatusCodes.Status201Created)]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserViewModel>> Register(RegisterUserRequest registerUser)
        {
            if (!this.ModelState.IsValid) return BadRequest(error: new { error = "Payload invalid" });

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (result.Errors.Any())
            {
                 return BadRequest(error: new { error = string.Join(";", result.Errors.Select(x => x.Description).ToArray())});
            }

            var userCreated = await _userManager.FindByEmailAsync(user.Email);
            await _userManager.AddToRoleAsync(userCreated, registerUser.Profile);

            await _signInManager.SignInAsync(user, false);

            return Ok(new RegisterUserViewModel(Guid.Parse(userCreated.Id), userCreated.Email));
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        [HttpPost("access")]
        public async Task<ActionResult<UserViewModel>> Login(LoginUserRequest loginUser)
        {
            if (!this.ModelState.IsValid) return BadRequest(error: new { error = "Payload invalid" });

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, false);

            if (result.Succeeded)
            {
                var token = await _userHandlerService.GetJwt(_userManager, _jwtSettings, loginUser.Email);
                var userResponse = await _userHandlerService.BuildUser(_service, _jwtSettings, loginUser.Email, token);
                return Ok(userResponse);
            }
            if (result.IsLockedOut)
            {
                return BadRequest(error: new { error = "This user is blocked" });
            }

            return NotFound("Invalid User/Password");
        }
    }
}
