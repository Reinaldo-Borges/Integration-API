using Integration.API.Extensions;
using Integration.API.Setup;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
builder.Services.AddFluentValidations();
builder.Services.ResolveDependencies(builder.Configuration);
builder.Services.BuildIdentityContext(builder.Configuration);

builder.Services.AddMvc(option => option.EnableEndpointRouting = false)
                .AddNewtonsoftJson(opt => {
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();