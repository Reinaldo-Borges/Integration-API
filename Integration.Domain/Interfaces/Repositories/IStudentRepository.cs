using Integration.Domain.Entities;
using Integration.Domain.Models;

namespace Integration.Domain.Interfaces.Repositories
{
    public interface IStudentRepository
	{
		Task Add(Student student);
		Task Update(Student student);
		Task<StudentModel> GetById(Guid id);
        Task<StudentModel> GetByUserId(Guid id);
        Task<StudentModel> GetByEmail(string email);
    }
}
