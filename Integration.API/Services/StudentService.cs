using Integration.Domain.Factories;
using Integration.Domain.Interfaces;
using Integration.Domain.Models;

namespace Integration.API.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentFactory _factory;

        public StudentService(IUnitOfWork unitOfWork, IStudentFactory studentFactory)
        {
            _unitOfWork = unitOfWork;
            _factory = studentFactory;
        }

        public async Task Add(IStudent student)
        {
            var studentBuilt = _factory.Builder(student);
            await _unitOfWork.StudentRepository.Add(studentBuilt);
            _unitOfWork.Commit();
        }

        public async Task<StudentModel> GetByEmail(string email)
        {
            var student = await _unitOfWork.StudentRepository.GetByEmail(email);
            return student;
        }

        public async Task<StudentModel> GetById(Guid id)
        {
            var student = await _unitOfWork.StudentRepository.GetById(id);
            return student;
        }

        public async Task<StudentModel> GetByUserId(Guid id)
        {
            var student = await _unitOfWork.StudentRepository.GetByUserId(id);
            return student;
        }

        public async Task Update(IStudent student)
        {
            var studentBuilt = _factory.Builder(student);
            await _unitOfWork.StudentRepository.Update(studentBuilt);
            _unitOfWork.Commit();
        }


    }
}