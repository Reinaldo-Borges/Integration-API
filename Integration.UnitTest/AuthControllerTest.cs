using System.Net;
using FluentAssertions;
using Integration.API.Controllers;
using Integration.API.Model;
using Integration.API.Model.Request;
using Integration.API.Model.ViewModel;
using Integration.API.Services;
using Integration.Domain.Enum;
using Integration.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace Integration.UnitTest
{
    public class AuthControllerTest
	{
        private readonly Mock<SignInManager<IdentityUser>> _signInManagerMok;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly Mock<IStudentService> _studentServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTest()
		{
            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            _signInManagerMok = new Mock<SignInManager<IdentityUser>>(_userManagerMock.Object, contextAccessorMock.Object, claimsFactoryMock.Object, null, null, null, null);

            _jwtSettings = Options.Create(new JwtSettings()
                        { Secret = "GRANDESCOISASFEZOSENHORPORNOSPORISSOSOMOSSALVOS",
                          ExpriresInHours = 10, Issuer = "MySystem", ValidIn = @"https://localhost" });

            _studentServiceMock = new Mock<IStudentService>(); 

            _authController = new AuthController(_signInManagerMok.Object, _userManagerMock.Object, _jwtSettings, _studentServiceMock.Object);
        }

        [Theory]
        [InlineData(TypeStudentEnum.Basic)]
        [InlineData(TypeStudentEnum.Partner)]
        [InlineData(TypeStudentEnum.Premium)]
        public void Register_User_SuccessOnRegister(TypeStudentEnum profile)
        {
            var registerRequest = new RegisterUserRequest()
            {
                Email = "john.doe@gmail.com",
                Password = "t3$T&,7221-Y",
                ConfirmPassword = "t3$T&,7221-Y",
                Profile = profile.ToString()
            };

            var user = new IdentityUser
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                EmailConfirmed = true
            };

            _userManagerMock
            .Setup(s => s.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

            _userManagerMock.Setup(s => s.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));

            _signInManagerMok.Setup(s => s.SignInAsync(user, false, null));          

            var actionResult = _authController.Register(registerRequest);

            _userManagerMock.Verify(v => v.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            _userManagerMock.Verify(v => v.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _userManagerMock.Verify(v => v.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            _signInManagerMok.Verify(v => v.SignInAsync(It.IsAny<IdentityUser>(), false, null), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(actionResult?.Result.Result);
            var result = Assert.IsAssignableFrom<RegisterUserViewModel>(okResult.Value);

            result.Email.Should().Be(registerRequest.Email);
            result.UserId.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void Register_UserDuplicated_ReturnBadRequest()
        {
            var registerRequest = new RegisterUserRequest()
            {
                Email = "john.doe@gmail.com",
                Password = "t3$T&,7221-Y",
                ConfirmPassword = "t3$T&,7221-Y",
                Profile = TypeStudentEnum.Basic.ToString(),
                CourseId = Guid.NewGuid()
            };

            var user = new IdentityUser
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                EmailConfirmed = true,
                Id = Guid.NewGuid().ToString()
            };

            var error = new IdentityError();
            error.Code = "DuplicateUserName";
            _userManagerMock.Setup(s => s.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(error));
            _userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            
            var actionResult = _authController.Register(registerRequest);


            _userManagerMock.Verify(v => v.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            var brResult = Assert.IsType<BadRequestObjectResult>(actionResult?.Result.Result);
            brResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Login_UserStudentRightData_SuccessAccess()
        {
            var userId = Guid.NewGuid();
            var userLogin = new LoginUserRequest()
            {
                Email = "john.doe@gmail.com",
                Password = "t3$T&,7221-Y"
            };

            var userViewModel = new UserViewModel()
            {
                Id = userId,
                Email = userLogin.Email
            };

            var userReturn = new IdentityUser
            {
                UserName = userLogin.Email,
                Email = userLogin.Email,
                EmailConfirmed = true,
                Id = userId.ToString()
            };

            var role = new List<string>() { "basic" };

            var studentModel = new StudentModel()
            {
                Email = userLogin.Email,
                Name = "John Doe",
                Id = Guid.NewGuid(),
                UserId = userId,
                CreationDate = DateTime.Now,
                TypeStudent = (int)TypeStudentEnum.Basic
            };

            _signInManagerMok.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(userReturn);
            _userManagerMock.Setup(s => s.GetRolesAsync(userReturn)).ReturnsAsync(role);

            _studentServiceMock.Setup(s => s.GetByEmail(It.IsAny<string>())).ReturnsAsync(studentModel);
        

            var actionResult = _authController.Login(userLogin);


            _signInManagerMok.Verify(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false), Times.Once);
            _userManagerMock.Verify(v => v.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _userManagerMock.Verify(v => v.GetRolesAsync(userReturn), Times.Once);

            _studentServiceMock.Verify(v => v.GetByEmail(It.IsAny<string>()), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(actionResult?.Result.Result);
            var result = Assert.IsAssignableFrom<UserViewModel>(okResult.Value);

            result.Token.Should().NotBeEmpty();
            result.Id.Should().Be(userViewModel.Id);
            result.Email.Should().Be(userViewModel.Email);
            result.TypeStudent.Should().Be(TypeStudentEnum.Basic);
        }

        [Fact]
        public void Login_UserWrongData_FailOnSignIn()
        {
            var userLogin = new LoginUserRequest()
            {
                Email = "john.doe@gmail.com",
                Password = ""
            };

            _signInManagerMok.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);


            var actionResult = _authController.Login(userLogin);


            _signInManagerMok.Verify(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false), Times.Once);

            var nfResult = Assert.IsType<NotFoundObjectResult>(actionResult?.Result.Result);
            nfResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            nfResult.Value.ToString().Should().Contain("Invalid User/Password");
        }

        [Fact]
        public void Login_BlockedUser_FailOnSignIn()
        {
            var userLogin = new LoginUserRequest()
            {
                Email = "john.doe@gmail.com",
                Password = "t3$T&,7221-Y"
            };

            _signInManagerMok.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);


            var actionResult = _authController.Login(userLogin);

            _signInManagerMok.Verify(v => v.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false), Times.Once);

            var brResult = Assert.IsType<BadRequestObjectResult>(actionResult?.Result.Result);
            brResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            brResult.Value.ToString().Should().Contain("This user is blocked");
        }

        [Fact]
        public void ExternalLogin_NewUser_SuccessOnExternailLogin()
        {
            var externaLogin = new ExternalUserRequest()
            {
                 Email = "john.doe@gmail.com",
                 Name = "John Doe"
            };

            IdentityUser? userReturn = null;

            var newUser = new IdentityUser
            {
                UserName = externaLogin.Email,
                Email = externaLogin.Email,
                EmailConfirmed = true
            };

            var role = new List<string>() { "Basic" };

            StudentModel? studentModel = null;         

            _userManagerMock.SetupSequence(s => s.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(userReturn).ReturnsAsync(newUser).ReturnsAsync(newUser);
            _userManagerMock.Setup(s => s.CreateAsync(newUser));
            _userManagerMock.Setup(s => s.AddToRoleAsync(newUser, "Basic"));
            _userManagerMock.Setup(s => s.GetRolesAsync(newUser)).ReturnsAsync(role);
            _studentServiceMock.Setup(s => s.GetByEmail(newUser.Email)).ReturnsAsync(studentModel);


            var actionResult = _authController.ExternalLogin(externaLogin);

            
            _userManagerMock.Verify(v => v.FindByEmailAsync(It.IsAny<string>()), Times.Exactly(3));
            _userManagerMock.Verify(v => v.CreateAsync(It.IsAny<IdentityUser>()), Times.Once);
            _userManagerMock.Verify(v => v.AddToRoleAsync(newUser, "Basic"), Times.Once);
            _userManagerMock.Verify(v => v.GetRolesAsync(newUser), Times.Once);
            _studentServiceMock.Verify(v => v.GetByEmail(newUser.Email), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(actionResult?.Result.Result);
            var result = Assert.IsAssignableFrom<UserViewModel>(okResult.Value);

            result.Token.Should().NotBeEmpty();          
            result.Email.Should().Be(newUser.Email);
            result.Name.Should().Be(string.Empty);
            result.Id.Should().NotBe(Guid.Empty);
            result.TypeStudent.Should().Be(TypeStudentEnum.Basic);
        }

        [Fact]
        public void ExternalLogin_ExistsUser_SuccessOnExternailLogin()
        {
            var externaLogin = new ExternalUserRequest()
            {
                Email = "john.doe@gmail.com",
                Name = "John Doe"
            };

            var user = new IdentityUser
            {
                UserName = externaLogin.Email,
                Email = externaLogin.Email,
                EmailConfirmed = true
            };

            var role = new List<string>() { "Basic" };

            var studentModel = new StudentModel()
            {
                Id = Guid.NewGuid(),
                Name = "Jhon Doe",
                Email = user.Email,
                Cellphone = "+5521788889999",
                Birthday = DateTime.Now,
                Country = "Brazil",
                TypeStudent = (int)TypeStudentEnum.Basic
            };

            _userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);        
            _userManagerMock.Setup(s => s.GetRolesAsync(user)).ReturnsAsync(role);
            _studentServiceMock.Setup(s => s.GetByEmail(user.Email)).ReturnsAsync(studentModel);


            var actionResult = _authController.ExternalLogin(externaLogin);


            _userManagerMock.Verify(v => v.FindByEmailAsync(It.IsAny<string>()), Times.Exactly(2));          
            _userManagerMock.Verify(v => v.GetRolesAsync(user), Times.Once);
            _studentServiceMock.Verify(v => v.GetByEmail(user.Email), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(actionResult?.Result.Result);
            var result = Assert.IsAssignableFrom<UserViewModel>(okResult.Value);

            result.Token.Should().NotBeEmpty();
            result.Id.Should().NotBe(Guid.Empty);
            result.Email.Should().Be(user.Email);
            result.Name.Should().Be(studentModel.Name);
            result.TypeStudent.Should().Be(TypeStudentEnum.Basic);
        }

        [Fact]
        public void ExternalLogin_UserWrongData_FailOnSignIn()
        {
            var userLogin = new ExternalUserRequest(){Email = "john.doe-gmail.com"};
            

            var actionResult = _authController.ExternalLogin(userLogin);


            var brResult = Assert.IsType<BadRequestObjectResult>(actionResult?.Result.Result);
            brResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            brResult.Value.ToString().Should().Contain("Invalid e-mail");
        }
    }
}

