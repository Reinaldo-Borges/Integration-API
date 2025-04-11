using System.Data;
using Dapper;
using Integration.Domain.Models;

namespace Integration.Infra.Data.Data
{
    public class StudentQuery
	{
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public StudentQuery(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }

        public async Task<StudentModel> GetById(Guid id)
        {
            var sql = @"SELECT TypeStudentId TypeStudent
                              ,Id
                              ,Document
                              ,Name     
                              ,Email
                              ,Cellphone
                              ,Birthday
                              ,UserId
                              ,CourseId
                              ,SecurityKey
                              ,CreationDate
                              ,Active
                          FROM Student With(NOLOCK)
                        WHERE Id = @Id";

            var parametros = new { Id = id };

            return await _connection.QueryFirstOrDefaultAsync<StudentModel>(sql, parametros, _transaction);
        }

        public async Task<StudentModel> GetByEmail(string email)
        {
            var sql = @"SELECT 
                               Id
                              ,Document
                              ,Name     
                              ,Email
                              ,Cellphone
                              ,Birthday
                              ,TypeStudentId TypeStudent
                              ,UserId
                              ,CourseId
                              ,SecurityKey
                              ,CreationDate
                              ,Active
                          FROM Student With(NOLOCK)
                        WHERE Email = @Email";

            var parametros = new { Email = email };

            return await _connection.QueryFirstOrDefaultAsync<StudentModel>(sql, parametros, _transaction);
        }

        public async Task<StudentModel> GetByUserId(Guid userId)
        {
            var sql = @"SELECT TypeStudentId TypeStudent
                              ,Id
                              ,Document
                              ,Name     
                              ,Email
                              ,Cellphone
                              ,Birthday
                              ,UserId
                              ,CourseId
                              ,SecurityKey
                              ,CreationDate
                              ,Active
                          FROM Student With(NOLOCK)
                        WHERE UserId = @UserId";

            var parametros = new { UserId = userId };

            return await _connection.QueryFirstOrDefaultAsync<StudentModel>(sql, parametros, _transaction);
        }
    }
}