using OnlineLibrary.Api.ExceptionHandlers;
using OnlineLibrary.Api.Extensions;
using OnlineLibrary.Api.Services;
using OnlineLibrary.Application.Abstractions;
using OnlineLibrary.Application.Extensions;
using OnlineLibrary.Persistence.Extensions;
using OnlineLibrary.Persistence.Interceptors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers().AddJsonDefaults();
builder.Services.AddDefaultCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.ConfigureSettings(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserProvider, HttpCurrentUserProvider>();
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddFileStorage();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
// app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
