using FluentValidation;
using Integration.Domain.Entities;

namespace Integration.Domain.Validator
{
    public class PartnerStudentValidator : AbstractValidator<PartnerStudent>
	{
		public PartnerStudentValidator()
		{           

            RuleFor(student => student.CourseId)
                .Equal(Guid.Empty).WithMessage("The property CourseId can't be empty.");
        }
	}
}

