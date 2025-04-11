using Integration.Domain.Entities;
using Integration.Domain.Interfaces;

namespace Integration.Domain.Factories
{
    public interface IStudentFactory
	{
        Student Builder(IStudent student);
	}
}