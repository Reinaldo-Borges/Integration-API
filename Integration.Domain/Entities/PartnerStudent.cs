using Integration.Domain.Enum;

namespace Integration.Domain.Entities
{
    public class PartnerStudent : Student
	{
        public override TypeStudentEnum StudentType => TypeStudentEnum.Partner;
        public Guid CourseId { get; private set; }

        public PartnerStudent(Guid courseId, string name, string email, Guid userId)
            :base(name, email, userId)
		{
            CourseId = courseId;
		}        
    }
}