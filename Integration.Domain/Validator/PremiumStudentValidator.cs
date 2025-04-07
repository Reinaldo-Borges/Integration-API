using FluentValidation;
using Integration.Domain.Entities;

namespace Integration.Domain.Validator
{
    public class PremiumStudentValidator : AbstractValidator<PremiumStudent>
    {
		public PremiumStudentValidator() 
		{		
			RuleFor(student => student.SecurityKey)
				.Equal(Guid.Empty).WithMessage("The property SecurityKey can't be empty.");        
		}
	}
}

