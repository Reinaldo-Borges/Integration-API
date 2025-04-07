using Integration.Domain.Enum;

namespace Integration.Domain.Entities
{
    public class BasicStudent: Student
	{
		public override TypeStudentEnum StudentType => TypeStudentEnum.Basic;

        public BasicStudent(string name, string email, Guid userId)
			:base(name, email, userId)
		{
		}

    }
}