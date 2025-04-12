using AutoMapper;
using Integration.API.Extensions;
using Integration.API.Model.Request;
using Integration.API.Model.ViewModel;
using Integration.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Integration.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : BaseController
    {
        private readonly IStudentService _service;
        private readonly IMapper _mapper;

        public StudentController(IStudentService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]     
        [HttpPost("new")]
        public async Task<ActionResult<StudentRequest>> Create(StudentRequest student)
        {
            if (!this.ModelState.IsValid) return BadRequest(error: new { error = "Payload invalid" });

            await _service.Add(student);          

            return Ok(student);           
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("edit")]
        public async Task<ActionResult<StudentRequest>> Update(StudentRequest student)
        {
            if (!this.ModelState.IsValid) return BadRequest(error: new { error = "Payload invalid" });

            await _service.Update(student);

            return Ok(student);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{email}/email")]
        public async Task<ActionResult<StudentViewModel>> GetByEmail(string email)
        {
            if (!email.IsValidEmail()) return BadRequest(error: new { error = "E-mail invalid" });

            var student = await _service.GetByEmail(email);

            if (student is null) return NotFound("Student not found");

            var viewModel = _mapper.Map<StudentViewModel>(student);

            return Ok(viewModel);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentViewModel>> GetById(Guid id)
        {
            if (!id.IsGuidNotEmpty()) return BadRequest(error: new { error = "Id is Guid.Empty" });

            var student = await _service.GetById(id);

            if (student is null) return NotFound("Student not found");

            var viewModel = _mapper.Map<StudentViewModel>(student);

            return Ok(viewModel);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<StudentViewModel>> GetByUserId(Guid userId)
        {
            if (!userId.IsGuidNotEmpty()) return BadRequest(error: new { error = "Id is Guid.Empty" });

            var student = await _service.GetByUserId(userId);

            if (student is null) return NotFound("Student not found");

            var viewModel = _mapper.Map<StudentViewModel>(student);

            return Ok(viewModel);
        }
    }
}