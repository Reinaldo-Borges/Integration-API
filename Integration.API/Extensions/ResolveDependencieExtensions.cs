using System.Data;
using Integration.API.Model;
using Integration.API.Services;
using Integration.Domain.Factories;
using Integration.Domain.Interfaces;
using Integration.Infra.Data.Infra.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace Integration.API.Extensions
{
    public static class ResolveDependencieExtensions
	{
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddScoped<JwtSettings>();

            services.AddScoped<IStudentService, StudentService>();

            services.AddScoped<IStudentFactory, StudentFactory>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();        

            services.AddScoped<IDbConnection>(x => new SqlConnection(configuration.GetConnectionString("SqlConnection")));

            return services;
        }
    }
}