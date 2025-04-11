using Integration.Domain.Enum;
using Integration.Domain.Validator;

namespace Integration.Domain.Entities
{
    public class PartnerStudent : Student
	{
        public override TypeStudentEnum TypeStudent => TypeStudentEnum.Partner;
        public override Guid CourseId { get; }

        public PartnerStudent(Guid courseId, string name, string email, Guid userId)
            :base(name, email, userId)
		{
            CourseId = courseId;

            Validate(this, new PartnerStudentValidator());
        }        
    }
}