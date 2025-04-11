using System;
using FluentValidation.AspNetCore;
using Integration.Domain.Validator;

namespace Integration.API.Extensions
{
	public static class FluentValidationExtensions
	{
		public static IServiceCollection AddFluentValidations(this IServiceCollection services)
		{
            services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<StudentValidator>());
            services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PartnerStudentValidator>());
            services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PremiumStudentValidator>());

            return services;
        }
    }
}

