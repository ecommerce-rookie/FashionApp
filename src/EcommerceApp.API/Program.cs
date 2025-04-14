using Application;
using Application.Middlewares;
using Domain.Constants;
using Infrastructure;
using Infrastructure.DocumentApi;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add middleware services to the container.
builder.Services.AddScoped<GlobalException>();
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddApplication();
builder.AddInfrastructure();
builder.AddPersistance();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseScalar();
}

app.UseMiddleware<GlobalException>();

//app.Use(async (context, next) =>
//{
//    try
//    {
//        if (context.Request.Path.StartsWithSegments("/swagger") &&
//        (!context.User.Identity?.IsAuthenticated ?? true))
//        {
//            await context.ChallengeAsync("oidc");
//            Console.WriteLine("User not authenticated, redirecting to login page.");
//            return;
//        }

//        Console.WriteLine("User authenticated, proceeding with request.");
//        await next();
//    } catch (Exception ex)
//    {
//        await context.Response.WriteAsync("An error occurred: " + ex.Message);
//    }
//});

app.UseCors(builder =>
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders(SystemConstant.HeaderPagination));

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//app.UseOpenApi();

app.Run();
