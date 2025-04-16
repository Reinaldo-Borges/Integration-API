using AutoMapper;
using FluentAssertions;
using Integration.API.Controllers;
using Integration.API.Model.Request;
using Integration.API.Model.ViewModel;
using Integration.API.Services;
using Integration.API.Setup;
using Integration.Domain.Enum;
using Integration.Domain.Interfaces;
using Integration.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Integration.UnitTest
{
    public class StudentControllerTest
    {
        private readonly Mock<IStudentService> _serviceMock;
        private readonly IMapper _mapper;
        private readonly StudentController _controller;

        public StudentControllerTest()
        {
            _serviceMock = new Mock<IStudentService>();
            var config = new MapperConfiguration(opt =>
            {
                opt.AddProfile(new AutoMapperConfig());
            });
            _mapper = config.CreateMapper();
            _controller = new StudentController(_serviceMock.Object, _mapper);
        }

        [Theory]
        [InlineData(TypeStudentEnum.Basic)]
        [InlineData(TypeStudentEnum.Partner)]
        [InlineData(TypeStudentEnum.Premium)]
        public void Create_StudentRightData_SuccessOnRegister(TypeStudentEnum typeStudent)
        {
            var student = new StudentRequest()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Email = "john.doe@gmail.com",
                Name = "John Doe",
                CourseId = Guid.NewGuid(),
                SecurityKey = Guid.NewGuid(),
                Document = "233.444.555.666",
                Birthday = DateTime.Now.AddYears(-20),
                Country = "USA",
                Cellphone = "1508333-4444",
                TypeStudent = typeStudent
            };

            _serviceMock.Setup(s => s.Add(It.IsAny<IStudent>()));


            var actionResult = _controller.Create(student);


            _serviceMock.Verify(v => v.Add(It.IsAny<IStudent>()), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(actionResult?.Result.Result);
            var result = Assert.IsAssignableFrom<StudentRequest>(okResult.Value);

            result.Email.Should().Be(student.Email);
            result.Id.Should().Be(student.Id);
            result.TypeStudent.Should().Be(typeStudent);
        }       

        [Fact]
        public void Update_StudentRightData_SuccessOnRegister()
        {
            var student = new StudentRequest()
            {
                Name = "John Stwart Doe",             
                Document = "233.444.555.666",
                Birthday = DateTime.Now.AddYears(-20),
                Country = "USA",
                Cellphone = "1508333-4444"
            };

            _serviceMock.Setup(s => s.Update(It.IsAny<IStudent>()));


            var actionResult = _controller.Update(student);


            _serviceMock.Verify(v => v.Update(It.IsAny<IStudent>()), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(actionResult?.Result.Result);
            var result = Assert.IsAssignableFrom<StudentRequest>(okResult.Value);

            result.Name.Should().Be(student.Name);
            result.Document.Should().Be(student.Document);
            result.Birthday.Should().Be(student.Birthday);
            result.Country.Should().Be(student.Country);
            result.Cellphone.Should().Be(student.Cellphone);
        }

        [Fact]
        public void Update_StudentWrongData_FailOnCretedStudent()
        {
            var student = new StudentRequest()
            {
                Id = Guid.Empty,
            };

            _controller.ModelState.AddModelError("key", "exception");
            var actionResult = _controller.Update(student);


            var brResult = Assert.IsType<BadRequestObjectResult>(actionResult?.Result.Result);

            brResult.Value.ToString().Should().Contain("Payload invalid");
        }

        [Fact]
        public void GetByEmail_RightData_ReturnStudentByEmail()
        {
            string email = "john.doe@gmail.com";

            var studentModel = new StudentModel()
            {
                Id = Guid.NewGuid(),
                Name = "Jhon Doe",
                Email = email,
                Cellphone = "+5521788889999",
                Birthday = DateTime.Now,
                Country = "Brazil",
                TypeStudent = (int)TypeStudentEnum.Basic
            };

            _serviceMock.Setup(s => s.GetByEmail(It.IsAny<string>())).ReturnsAsync(studentModel);


            var actionResult = _controller.GetByEmail(email);


            _serviceMock.Verify(v => v.GetByEmail(It.IsAny<string>()), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(actionResult?.Result.Result);
            var result = Assert.IsAssignableFrom<StudentViewModel>(okResult.Value);

            result.Name.Should().Be(studentModel.Name);
            result.Document.Should().Be(studentModel.Document);
            result.Birthday.Should().Be(studentModel.Birthday);
            result.Country.Should().Be(studentModel.Country);
            result.Cellphone.Should().Be(studentModel.Cellphone);
        }

        [Fact]
        public void GetByEmail_WrongData_ReturnBadRequest()
        {
            string email = "john.doe-gmail.com";


            var actionResult = _controller.GetByEmail(email);


            var brResult = Assert.IsType<BadRequestObjectResult>(actionResult?.Result.Result);
            brResult.Value.ToString().Should().Contain("E-mail invalid");        
        }

        [Fact]
        public void GetByEmail_WrongStudent_ReturnNotFound()
        {
            string email = "john.doe@gmail.com";

            StudentModel? studentModel = null;

            _serviceMock.Setup(s => s.GetByEmail(It.IsAny<string>())).ReturnsAsync(studentModel);


            var actionResult = _controller.GetByEmail(email);


            _serviceMock.Verify(v => v.GetByEmail(It.IsAny<string>()), Times.Once);

            var nfResult = Assert.IsType<NotFoundObjectResult>(actionResult?.Result.Result);
            nfResult.Value.Should().Be("Student not found");
        }

        [Fact]
        public void GetById_RightData_ReturnStudentById()
        {
            var studentId = Guid.NewGuid();

            var studentModel = new StudentModel()
            {
                Id = studentId,
                Name = "Jhon Doe",
                Email = "john.doe@gmail.com",
                Cellphone = "+5521788889999",
                Birthday = DateTime.Now,
                Country = "Brazil",
                TypeStudent = (int)TypeStudentEnum.Basic
            };

            _serviceMock.Setup(s => s.GetById(It.IsAny<Guid>())).ReturnsAsync(studentModel);


            var actionResult = _controller.GetById(studentId);


            _serviceMock.Verify(v => v.GetById(It.IsAny<Guid>()), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(actionResult?.Result.Result);
            var result = Assert.IsAssignableFrom<StudentViewModel>(okResult.Value);

            result.Id.Should().Be(studentModel.Id);
            result.Email.Should().Be(studentModel.Email);
            result.Name.Should().Be(studentModel.Name);
            result.Document.Should().Be(studentModel.Document);
            result.Birthday.Should().Be(studentModel.Birthday);
            result.Country.Should().Be(studentModel.Country);
            result.Cellphone.Should().Be(studentModel.Cellphone);
        }

        [Fact]
        public void GetById_WrongData_ReturnBadRequest()
        {
            var studentId = Guid.Empty;


            var actionResult = _controller.GetById(studentId);


            var brResult = Assert.IsType<BadRequestObjectResult>(actionResult?.Result.Result);
            brResult.Value.ToString().Should().Contain("Id is Guid.Empty");
        }

        [Fact]
        public void GetById_WrongStudent_ReturnNotFound()
        {
            var studentId = Guid.NewGuid();

            StudentModel? studentModel = null;

            _serviceMock.Setup(s => s.GetById(It.IsAny<Guid>())).ReturnsAsync(studentModel);


            var actionResult = _controller.GetById(studentId);


            _serviceMock.Verify(v => v.GetById(It.IsAny<Guid>()), Times.Once);

            var nfResult = Assert.IsType<NotFoundObjectResult>(actionResult?.Result.Result);
            nfResult.Value.Should().Be("Student not found");
        }

        [Fact]
        public void GetByUserId_RightData_ReturnStudentByUserId()
        {
            var userId = Guid.NewGuid();

            var studentModel = new StudentModel()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Jhon Doe",
                Email = "john.doe@gmail.com",
                Cellphone = "+5521788889999",
                Birthday = DateTime.Now,
                Country = "Brazil",
                TypeStudent = (int)TypeStudentEnum.Basic
            };

            _serviceMock.Setup(s => s.GetByUserId(It.IsAny<Guid>())).ReturnsAsync(studentModel);


            var actionResult = _controller.GetByUserId(userId);


            _serviceMock.Verify(v => v.GetByUserId(It.IsAny<Guid>()), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(actionResult?.Result.Result);
            var result = Assert.IsAssignableFrom<StudentViewModel>(okResult.Value);

            result.Id.Should().Be(studentModel.Id);
            result.UserId.Should().Be(studentModel.UserId);
            result.Email.Should().Be(studentModel.Email);
            result.Name.Should().Be(studentModel.Name);
            result.Document.Should().Be(studentModel.Document);
            result.Birthday.Should().Be(studentModel.Birthday);
            result.Country.Should().Be(studentModel.Country);
            result.Cellphone.Should().Be(studentModel.Cellphone);
        }

        [Fact]
        public void GetByUserId_WrongData_ReturnBadRequest()
        {
            var userId = Guid.Empty;


            var actionResult = _controller.GetByUserId(userId);


            var brResult = Assert.IsType<BadRequestObjectResult>(actionResult?.Result.Result);
            brResult.Value.ToString().Should().Contain("Id is Guid.Empty");
        }

        [Fact]
        public void GetByUserId_WrongStudent_ReturnNotFound()
        {
            var userId = Guid.NewGuid();

            StudentModel? studentModel = null;

            _serviceMock.Setup(s => s.GetByUserId(It.IsAny<Guid>())).ReturnsAsync(studentModel);


            var actionResult = _controller.GetByUserId(userId);


            _serviceMock.Verify(v => v.GetByUserId(It.IsAny<Guid>()), Times.Once);

            var nfResult = Assert.IsType<NotFoundObjectResult>(actionResult?.Result.Result);
            nfResult.Value.Should().Be("Student not found");
        }
    }
}

