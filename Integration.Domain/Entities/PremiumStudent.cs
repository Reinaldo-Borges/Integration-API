using Integration.Domain.Enum;
using Integration.Domain.Validator;

namespace Integration.Domain.Entities
{
    public class PremiumStudent : Student
	{
		public override TypeStudentEnum TypeStudent => TypeStudentEnum.Premium;
		public override Guid SecurityKey { get; }

		public PremiumStudent(Guid securityKey, string name, string email, Guid userId)
            : base(name, email, userId)
        {
            SecurityKey = securityKey;

            Validate(this, new PremiumStudentValidator());
        }

    }
}