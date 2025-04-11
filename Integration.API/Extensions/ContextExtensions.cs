using Integration.API.Model;
using Integration.Infra.Data.Infra.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Integration.API.Extensions
{
    public static class ContextExtensions
	{
        public static IServiceCollection BuildIdentityContext(this IServiceCollection services, IConfiguration configuration)
        {
            //var migrationAssembly = typeof(Program).GetType().Assembly.GetName().Name;
            services.AddDbContext<IdentityContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
                   // sql => sql.MigrationsAssembly(migrationAssembly));

            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();
            
            
            var jwtSettingsSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSettingsSection);

            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.ValidIn,
                    ValidIssuer = jwtSettings.Issuer
                };
            });

            return services;
        }
    }
}

