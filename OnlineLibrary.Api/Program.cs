using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Application.Extensions;
using OnlineLibrary.Persistence;
using OnlineLibrary.Persistence.Extensions;
using OnlineLibrary.Persistence.Interceptors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
