using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Integration.API.Model.Request;
using Integration.API.Model.ViewModel;
using Integration.Domain.Enum;
using Integration.IntegrationTest.Builder;
using Integration.IntegrationTest.Setup;

namespace Integration.IntegrationTest
{
    public class AuthControllerTest : BaseTest, IClassFixture<APIAppFactory>
    {
        private readonly APIAppFactory _apiAppFactory;

        public AuthControllerTest(APIAppFactory apiAppFactory)
            :base(apiAppFactory)
        {
            _apiAppFactory = apiAppFactory;
        }


        [Theory]
        [InlineData(TypeStudentEnum.Basic)]
        [InlineData(TypeStudentEnum.Partner)]
        [InlineData(TypeStudentEnum.Premium)]
        public async Task Register_UserRightData_SuccessOnRegisterUser(TypeStudentEnum profile)
        {
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();

            var newUser = new RegisterUserRequest()
            {
                Email = "john.doe@doxcise.com",
                Password = "t3$T&!NteGr@d0",
                ConfirmPassword = "t3$T&!NteGr@d0",             
                Profile = profile.ToString(),
                CourseId = Guid.NewGuid()                
            };

            var body = JsonSerializer.Serialize(newUser);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync("/api/Auth/register", stringContent);


            var query = "SELECT Email FROM AspNetUsers WHERE Email = @Email";
            var userCreated = await _apiAppFactory.ExecuteSingleAsync<RegisterUserViewModel>(query, new { Email = newUser.Email });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            userCreated.Email.Should().Be(newUser.Email);
        }

        [Fact]
        public async Task Register_DuplicatedUser_ReturnBadRequest()
        {
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();

            var userId = Guid.NewGuid();

            var password = "t3$T&!NteGr@d0";
            var userBuilder = new UserBuilder().Build(userId, "John.doe@gmail.com", password);
            await CreatedUser(userBuilder);
            await CreatedUserRole(userId, (int)TypeStudentEnum.Basic);

            var newUser = new RegisterUserRequest()
            {
                Email = userBuilder.Email,
                Password = "t3$T&!NteGr@d0",
                ConfirmPassword = "t3$T&!NteGr@d0",
                Profile = TypeStudentEnum.Basic.ToString(),
                CourseId = Guid.Empty
            };

            var body = JsonSerializer.Serialize(newUser);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync("/api/Auth/register", stringContent);


            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        [Fact]
        public async Task Login_RightData_SuccessOnGetUser()
        {
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();

            var userId = Guid.NewGuid();

            var password = "t3$T&!NteGr@d0";
            var userBuilder = new UserBuilder().Build(userId, "John.doe@gmail.com", password);
            await CreatedUser(userBuilder);
            await CreatedUserRole(userId, (int)TypeStudentEnum.Basic);

            var login = new LoginUserRequest()
            {
                Email = userBuilder.Email,
                Password = password
            };

            var body = JsonSerializer.Serialize(login);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync("/api/Auth/access", stringContent);


            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ExternalLogin_NeuUser_CreateAndGetUser()
        {
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();           

            var login = new ExternalUserRequest()
            {
                Email = "John.doe@gmail.com"
            };

            var body = JsonSerializer.Serialize(login);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync("/api/Auth/access/external", stringContent);                                                   


            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ExternalLogin_RegisteredUser_GetUser()
        {
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();

            var userId = Guid.NewGuid();

            var password = "t3$T&!NteGr@d0";
            var userBuilder = new UserBuilder().Build(userId, "John.doe@gmail.com", password);
            await CreatedUser(userBuilder);
            await CreatedUserRole(userId, (int)TypeStudentEnum.Basic);

            var login = new ExternalUserRequest()
            {
                Email = "John.doe@gmail.com"
            };

            var body = JsonSerializer.Serialize(login);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync("/api/Auth/access/external", stringContent);


            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

