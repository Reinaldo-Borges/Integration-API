using System.Data;
using Integration.Domain.Entities;
using Integration.Domain.Interfaces.Repositories;
using Integration.Domain.Models;
using Integration.Infra.Data.Data;

namespace Integration.Infra.Data.Infra.Repository
{
    public class StudentRepository : IStudentRepository
	{
        private IDbTransaction _transaction;        

        public StudentRepository(IDbTransaction transaction)
		{
            _transaction = transaction;          
		}

        public async Task Add(Student student)
        {
            await new StudentCommand(_transaction).Add(student);
        }

        public async Task Update(Student student)
        {
            await new StudentCommand(_transaction).Update(student);
        }

        public async Task<StudentModel> GetByEmail(string email)
        {
            return await new StudentQuery(_transaction).GetByEmail(email);            
        }

        public async Task<StudentModel> GetById(Guid id)
        {
            return await new StudentQuery(_transaction).GetById(id);
        }

        public async Task<StudentModel> GetByUserId(Guid id)
        {
            return await new StudentQuery(_transaction).GetByUserId(id);
        }

    }
}