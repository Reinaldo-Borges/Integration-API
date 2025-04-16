using System.Data;
using Dapper;
using Integration.Domain.Entities;

namespace Integration.Infra.Data.Data
{
    public class StudentCommand
	{
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public StudentCommand(IDbTransaction transaction)
		{
			_transaction = transaction;
			_connection = _transaction.Connection;
		}

        public async Task Add(Student student)
        {                
            var sql = @"INSERT INTO Student
                                (TypeStudentId, Id, Name, Email, Document,
                                Cellphone, Country, Birthday, UserId, CourseId, SecurityKey)
                            VALUES
                                (@TypeStudentId, @Id, @Name, @Email, @Document,
                                @Cellphone, @Country, @Birthday, @UserId, @CourseId, @SecurityKey)";

            var parametros = new
            {
                TypeStudentId = (int) student.TypeStudent,
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Document = student.Document,
                Cellphone = student.Cellphone,
                Country = student.Country,
                Birthday = student.Birthday,
                UserId = student.UserId,
                CourseId = student.CourseId,
                SecurityKey = student.SecurityKey              
            };

            await _connection.ExecuteAsync(sql, parametros, _transaction);            
        }

        public async Task Update(Student student)
        {
            var sql = @"UPDATE Student
                            SET   Name = @Name, Document = @Document, Cellphone = @Cellphone, Country = @Country, Birthday = @Birthday
                        WHERE Id = @Id";

            var parametros = new
            {
                Id = student.Id,
                Name = student.Name,         
                Document = student.Document,
                Cellphone = student.Cellphone,
                Country = student.Country,
                Birthday = student.Birthday              
            };

            await _connection.ExecuteAsync(sql, parametros, _transaction);   
        }
    }
}