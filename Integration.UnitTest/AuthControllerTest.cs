using System;
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
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
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
        private readonly Mock<IUserHandlerService> _userHandlerServiceMock;
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
            _userHandlerServiceMock = new Mock<IUserHandlerService>();


            _authController = new AuthController(_signInManagerMok.Object, _userManagerMock.Object, _jwtSettings,
                                                    _studentServiceMock.Object, _userHandlerServiceMock.Object);
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

    }
}

