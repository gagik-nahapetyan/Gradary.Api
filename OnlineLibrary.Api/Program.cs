using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OnlineLibrary.Api.ExceptionHandlers;
using OnlineLibrary.Application.Extensions;
using OnlineLibrary.Persistence;
using OnlineLibrary.Persistence.Extensions;
using OnlineLibrary.Persistence.Interceptors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Online Library API",
        Version = "v1",
        Description = "REST API for managing books, authors, categories, users and reviews."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddDbContext<OnlineLibraryDbContext>(
    options => options
        .UseSqlServer(builder.Configuration["ConnectionStrings:OnlineLibraryDb"])
        .AddInterceptors(new AuditInterceptor())
    );

builder.Services.AddServices();
builder.Services.AddRepositories();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
