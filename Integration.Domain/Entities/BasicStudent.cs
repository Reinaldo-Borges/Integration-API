using Integration.Domain.Enum;
using Integration.Domain.Validator;

namespace Integration.Domain.Entities
{
    public class BasicStudent: Student
	{
		public override TypeStudentEnum TypeStudent => TypeStudentEnum.Basic;

        public BasicStudent(string name, string email, Guid userId)
			:base(name, email, userId)
		{
            
        }

    }
}