using System.Data;
using Integration.Domain.Interfaces;
using Integration.Domain.Interfaces.Repositories;
using Integration.Infra.Data.Infra.Repository;

namespace Integration.Infra.Data.Infra.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    { 
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IStudentRepository _studentRepository;
        private bool _disposed;

        public UnitOfWork(IDbConnection connection)
		{
            _connection = connection;
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public IStudentRepository StudentRepository =>
            _studentRepository ??= _studentRepository = new StudentRepository(_transaction);


        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
            _disposed = true;
        }          

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}