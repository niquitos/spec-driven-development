using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application;
using TaskTracker.Api.Middleware;
using TaskTracker.Infrastructure;
using TaskTracker.Infrastructure.Persistence;

[assembly: InternalsVisibleTo("TaskTracker.IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()  // For development
              .AllowAnyMethod()
              .AllowAnyHeader();
        // For production, specify your frontend URL:
        // policy.WithOrigins("http://localhost:3000")
        //       .AllowAnyMethod()
        //       .AllowAnyHeader();
    });
});

// Add services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try
    {
         await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating.");
        throw;
    }
}

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowFrontend");
app.UseExceptionMiddleware();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
