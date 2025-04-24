using Microsoft.AspNetCore.CookiePolicy;
using StackExchange.Redis;
using StoreFront.Application.Middlewares;
using StoreFront.Application.Services;
using StoreFront.Application.Services.CartService;
using StoreFront.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorPages();

builder.Services.AddSession();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddAuthenticationConfig();

builder.Services.AddHttpConfig(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.AddRedis();

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<SafeApiCaller>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<CheckUserProfileMiddleware>();

app.MapRazorPages();

app.Run();
