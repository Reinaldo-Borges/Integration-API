using System.Data;
using Dapper;
using Integration.Infra.Data.Infra.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Respawn;
using Testcontainers.MsSql;

namespace Integration.IntegrationTest.Setup
{
    public class APIAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _sqlServerContainer = new MsSqlBuilder()               
           .Build();


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {  
            builder.ConfigureServices(services =>
            { 
                var containerContext = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<IdentityContext>));

                var oldconnection = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IDbConnection));

                services.Remove(containerContext!);
                services.Remove(oldconnection!);

                services.AddScoped<IDbConnection>(x => new SqlConnection(GetCustomConnection()));

                services.AddDbContext<IdentityContext>(options =>
                {
                    options.UseSqlServer(GetCustomConnection());                    
                    
                });

                //--
                services.Configure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                        options =>
                        {
                            options.Configuration = new OpenIdConnectConfiguration
                            {
                                Issuer = JwtTokenProvider.Issuer,
                            };                            
                            options.TokenValidationParameters.ValidIssuer = JwtTokenProvider.Issuer;
                            options.TokenValidationParameters.ValidAudience = JwtTokenProvider.Issuer;
                            options.Configuration.SigningKeys.Add(JwtTokenProvider.SecurityKey);
                        }
                );

            });
          
        }      

        public async Task InitializeAsync()
        {
            await _sqlServerContainer.StartAsync();

            await CreatedCustomDataBase();

            await AddScript();      
        }

        async Task IAsyncLifetime.DisposeAsync()
        {           
            await _sqlServerContainer.StopAsync();
        }

        public async Task CreatedCustomDataBase()
        {        
            using (SqlConnection connection = new SqlConnection(_sqlServerContainer.GetConnectionString()))
            {
                await connection.ExecuteAsync("CREATE DATABASE integrationdbtest");
            }
        }

        public async Task AddScript()
        {            
            var script = File.ReadAllText(Path.GetFullPath("Script/initdb.sql")).Replace("\n", " ").Replace("\t", " ");
            using (SqlConnection connection = new SqlConnection(GetCustomConnection()))
            {
                SqlCommand command = new SqlCommand(script, connection);
                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }

        private string GetCustomConnection()
        {
            var connectionString = new SqlConnectionStringBuilder(_sqlServerContainer.GetConnectionString());
            connectionString.InitialCatalog = "integrationdbtest";

            return connectionString.ToString();
        }

        public async Task ExecuteCommandAsync(string command, object? parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(GetCustomConnection()))
            {
                await connection.ExecuteAsync(command, parameters);
            }
        }

        public async Task<T> ExecuteSingleAsync<T>(string query, object parameters) where T : class, new()
        {
            await using (var connection = new SqlConnection(GetCustomConnection()))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
            }
        }

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query, object parameters) where T : class, new()
        {
            await using (var connection = new SqlConnection(GetCustomConnection()))
            {
                return await connection.QueryAsync<T>(query, parameters);
            }
        }

        public async Task<T> ExecuteEscalarAsync<T>(string query, object parameters)
        {
            await using (var connection = new SqlConnection(GetCustomConnection()))
            {
                return await connection.ExecuteScalarAsync<T>(query, parameters);
            }
        }

        public async Task ClearDatabaseAsync()
        {
            using var connection = new SqlConnection(GetCustomConnection());
            await connection.OpenAsync();
            var respawner = await Respawner.CreateAsync(connection);
            await respawner.ResetAsync(connection);
        }
    }
}

