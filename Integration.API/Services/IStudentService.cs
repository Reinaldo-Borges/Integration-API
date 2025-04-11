using Integration.Domain.Entities;
using Integration.Domain.Interfaces;
using Integration.Domain.Models;

namespace Integration.API.Services
{
    public interface IStudentService
	{
        Task Add(IStudent student);
        Task Update(IStudent student);
        Task<StudentModel> GetById(Guid id);
        Task<StudentModel> GetByUserId(Guid id);
        Task<StudentModel> GetByEmail(string email);
    }
}