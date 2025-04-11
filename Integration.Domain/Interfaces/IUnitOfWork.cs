using Integration.Domain.Interfaces.Repositories;

namespace Integration.Domain.Interfaces
{
    public interface IUnitOfWork
	{
        IStudentRepository StudentRepository { get; }
      
        void Commit();       
    }
}