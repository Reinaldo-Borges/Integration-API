using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Integration.API.Model.Request;
using Integration.Domain.Enum;
using Integration.Domain.Models;
using Integration.IntegrationTest.Builder;
using Integration.IntegrationTest.Setup;

namespace Integration.IntegrationTest
{
    public class StudentControllerTest : BaseTest, IClassFixture<APIAppFactory>
    {
        private readonly APIAppFactory _apiAppFactory;

        public StudentControllerTest(APIAppFactory apiAppFactory)
            : base(apiAppFactory)
        {
            _apiAppFactory = apiAppFactory;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }


        [Fact]
        public async Task Create_Student_CreatedStudent()
        {            
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();

            var userId = Guid.NewGuid();
            var password = "t3$T&!NteGr@d0";
            var userBuilder = new UserBuilder().Build(userId, "John.doe@gmail.com", password);
            await CreatedUser(userBuilder);
            await CreatedUserRole(userId, (int)TypeStudentEnum.Basic);
            await CreatedTypeStudent();

            var newStudent = new StudentRequest()
            {
                Id = Guid.NewGuid(),
                Email = userBuilder.Email,
                Name = "John Doe",
                TypeStudent = TypeStudentEnum.Basic,
                Document = "1231232211312",
                Cellphone = "15083334577",
                Country = "USA",
                UserId = userId,
                CourseId = Guid.NewGuid(),
                SecurityKey = Guid.NewGuid()
            };

            var body = JsonSerializer.Serialize(newStudent);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");


            var response = await _client.PostAsync("/api/Student/new", stringContent);


            var query = "SELECT Name FROM Student s WHERE s.Id = @Id";
            var student = await _apiAppFactory.ExecuteQueryAsync<StudentModel>(query, new
            {
                Id = newStudent.Id
            });

            response.EnsureSuccessStatusCode();
            student.FirstOrDefault().Name.Should().Be(newStudent.Name);
        }

        [Fact]
        public async Task Update_Student_UpdatedStudent()
        {
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();

            var userId = Guid.NewGuid();
            var password = "t3$T&!NteGr@d0";
            var userBuilder = new UserBuilder().Build(userId, "John.doe@gmail.com", password);
            await CreatedUser(userBuilder);
            await CreatedUserRole(userId, (int) TypeStudentEnum.Basic);
            await CreatedTypeStudent();

            var newStudent = new StudentRequest()
            {
                Id = Guid.NewGuid(),
                Email = userBuilder.Email,
                Name = "John Doe",
                TypeStudent = TypeStudentEnum.Basic,
                Document = "1231232211312",
                Cellphone = "15083334577",
                Country = "USA",
                UserId = userId,
                CourseId = Guid.NewGuid(),
                SecurityKey = Guid.NewGuid()
            };

            var command = @"INSERT INTO Student
                                    (TypeStudentId, Id, Name, Email, Document,
                                    Cellphone, Country, Birthday, UserId, CourseId, SecurityKey)
                                VALUES
                                    (@TypeStudentId, @Id, @Name, @Email, @Document,
                                    @Cellphone, @Country, @Birthday, @UserId, @CourseId, @SecurityKey)";

            var parametros = new
            {
                TypeStudentId = (int)newStudent.TypeStudent,
                Id = newStudent.Id,
                Name = newStudent.Name,
                Email = newStudent.Email,
                Document = newStudent.Document,
                Cellphone = newStudent.Cellphone,
                Country = newStudent.Country,
                Birthday = newStudent.Birthday,
                UserId = newStudent.UserId,
                CourseId = newStudent.CourseId,
                SecurityKey = newStudent.SecurityKey
            };

            await _apiAppFactory.ExecuteCommandAsync(command, parametros);

            var updateStudent = new StudentRequest()
            {
                Id = newStudent.Id,
                Name = "John Swart Doe",
                Email = newStudent.Email,
                Document = "123123123123123",
                Cellphone = "15083334111",
                Country = "USA",
                Birthday = DateTime.Now.AddYears(-20),
                TypeStudent = newStudent.TypeStudent,
                UserId = newStudent.UserId

            };

            var body = JsonSerializer.Serialize(updateStudent);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");


            var response = await _client.PatchAsync("/api/Student/edit", stringContent);


            var query = "SELECT Name, Document FROM Student s WHERE s.Id = @Id";
            var student = await _apiAppFactory.ExecuteQueryAsync<StudentModel>(query, new {Id = updateStudent.Id});

            response.EnsureSuccessStatusCode();
            student.FirstOrDefault().Name.Should().Be(updateStudent.Name);
            student.FirstOrDefault().Document.Should().Be(updateStudent.Document);
        }

        [Fact]
        public async Task GetByEmail_RightData_GetStudent()
        {
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();

            var userId = Guid.NewGuid();
            var password = "t3$T&!NteGr@d0";
            var userBuilder = new UserBuilder().Build(userId, "John.doe@gmail.com", password);
            await CreatedUser(userBuilder);
            await CreatedUserRole(userId, (int)TypeStudentEnum.Basic);
            await CreatedTypeStudent();

            var newStudent = new StudentRequest()
            {
                Id = Guid.NewGuid(),
                Email = userBuilder.Email,
                Name = "John Doe",
                TypeStudent = TypeStudentEnum.Basic,
                Document = "1231232211312",
                Cellphone = "15083334577",
                Country = "USA",
                UserId = userId,
                CourseId = Guid.NewGuid(),
                SecurityKey = Guid.NewGuid()
            };

            var command = @"INSERT INTO Student
                                    (TypeStudentId, Id, Name, Email, Document,
                                    Cellphone, Country, Birthday, UserId, CourseId, SecurityKey)
                                VALUES
                                    (@TypeStudentId, @Id, @Name, @Email, @Document,
                                    @Cellphone, @Country, @Birthday, @UserId, @CourseId, @SecurityKey)";

            var parametros = new
            {
                TypeStudentId = (int)newStudent.TypeStudent,
                Id = newStudent.Id,
                Name = newStudent.Name,
                Email = newStudent.Email,
                Document = newStudent.Document,
                Cellphone = newStudent.Cellphone,
                Country = newStudent.Country,
                Birthday = newStudent.Birthday,
                UserId = newStudent.UserId,
                CourseId = newStudent.CourseId,
                SecurityKey = newStudent.SecurityKey
            };

            await _apiAppFactory.ExecuteCommandAsync(command, parametros);


            var response = await _client.GetAsync($"/api/Student/{newStudent.Email}/email");


            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_RightData_GetStudent()
        {
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();

            var userId = Guid.NewGuid();
            var password = "t3$T&!NteGr@d0";
            var userBuilder = new UserBuilder().Build(userId, "John.doe@gmail.com", password);
            await CreatedUser(userBuilder);
            await CreatedUserRole(userId, (int)TypeStudentEnum.Basic);
            await CreatedTypeStudent();

            var newStudent = new StudentRequest()
            {
                Id = Guid.NewGuid(),
                Email = userBuilder.Email,
                Name = "John Doe",
                TypeStudent = TypeStudentEnum.Basic,
                Document = "1231232211312",
                Cellphone = "15083334577",
                Country = "USA",
                UserId = userId,
                CourseId = Guid.NewGuid(),
                SecurityKey = Guid.NewGuid()
            };

            var command = @"INSERT INTO Student
                                    (TypeStudentId, Id, Name, Email, Document,
                                    Cellphone, Country, Birthday, UserId, CourseId, SecurityKey)
                                VALUES
                                    (@TypeStudentId, @Id, @Name, @Email, @Document,
                                    @Cellphone, @Country, @Birthday, @UserId, @CourseId, @SecurityKey)";

            var parametros = new
            {
                TypeStudentId = (int)newStudent.TypeStudent,
                Id = newStudent.Id,
                Name = newStudent.Name,
                Email = newStudent.Email,
                Document = newStudent.Document,
                Cellphone = newStudent.Cellphone,
                Country = newStudent.Country,
                Birthday = newStudent.Birthday,
                UserId = newStudent.UserId,
                CourseId = newStudent.CourseId,
                SecurityKey = newStudent.SecurityKey
            };

            await _apiAppFactory.ExecuteCommandAsync(command, parametros);


            var response = await _client.GetAsync($"/api/Student/{newStudent.Id}");


            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetByUserId_RightData_GetStudent()
        {
            await _apiAppFactory.ClearDatabaseAsync();

            await CreatedRoles();

            var userId = Guid.NewGuid();
            var password = "t3$T&!NteGr@d0";
            var userBuilder = new UserBuilder().Build(userId, "John.doe@gmail.com", password);
            await CreatedUser(userBuilder);
            await CreatedUserRole(userId, (int)TypeStudentEnum.Basic);
            await CreatedTypeStudent();

            var newStudent = new StudentRequest()
            {
                Id = Guid.NewGuid(),
                Email = userBuilder.Email,
                Name = "John Doe",
                TypeStudent = TypeStudentEnum.Basic,
                Document = "1231232211312",
                Cellphone = "15083334577",
                Country = "USA",
                UserId = userId,
                CourseId = Guid.NewGuid(),
                SecurityKey = Guid.NewGuid()
            };

            var command = @"INSERT INTO Student
                                    (TypeStudentId, Id, Name, Email, Document,
                                    Cellphone, Country, Birthday, UserId, CourseId, SecurityKey)
                                VALUES
                                    (@TypeStudentId, @Id, @Name, @Email, @Document,
                                    @Cellphone, @Country, @Birthday, @UserId, @CourseId, @SecurityKey)";

            var parametros = new
            {
                TypeStudentId = (int)newStudent.TypeStudent,
                Id = newStudent.Id,
                Name = newStudent.Name,
                Email = newStudent.Email,
                Document = newStudent.Document,
                Cellphone = newStudent.Cellphone,
                Country = newStudent.Country,
                Birthday = newStudent.Birthday,
                UserId = newStudent.UserId,
                CourseId = newStudent.CourseId,
                SecurityKey = newStudent.SecurityKey
            };

            await _apiAppFactory.ExecuteCommandAsync(command, parametros);


            var response = await _client.GetAsync($"/api/Student/user/{newStudent.UserId}");


            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}

