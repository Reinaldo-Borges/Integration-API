using FluentValidation;
using Integration.Domain.Entities;

namespace Integration.Domain.Validator
{
    public class StudentValidator : AbstractValidator<Student>
	{
		public StudentValidator()
		{
			RuleFor(student => student.Name)
				.NotEmpty().WithMessage("The property Name can't be void");
            RuleFor(student => student.Email)
				.NotEmpty().WithMessage("The property Email can't be void")
				.EmailAddress().WithMessage("Invalid e-mail.");
			RuleFor(student => student.UserId)
				.NotEqual(Guid.Empty).WithMessage("The property UserId can't be empty.");
		}
		
	}
}