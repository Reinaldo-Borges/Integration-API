using Integration.Domain.Enum;

namespace Integration.Domain.Entities
{
    public class PremiumStudent : Student
	{
		public override TypeStudentEnum StudentType => TypeStudentEnum.Premium;
		public Guid SecurityKey { get; private set; }

		public PremiumStudent(Guid securityKey, string name, string email, Guid userId)
            : base(name, email, userId)
        {
            SecurityKey = securityKey;
        }

    }
}