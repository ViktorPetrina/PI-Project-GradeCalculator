using GradeCalculator.Adapter;
using GradeCalculator.AutoMapper;
using GradeCalculator.Models;
using GradeCalculator.Repository;
using GradeCalculator.Security;
using GradeCalculator.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PiGradeCalculatorContext>(options => {
    options.UseSqlServer("name=ConnectionStrings:connection");
});
builder.Services.AddScoped<StatistikaService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<KorisnikService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IReadAllRepository<Ocjena>, ComplexOcjenaRepository>();
builder.Services.AddScoped<IRepository<Predmet>, PredmetRepository>();
builder.Services.AddScoped<IRepository<Godina> ,GodinaRepository>();

builder.Services.AddScoped<IKorisnikAdapter, KorisnikAdapter>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});//mico: za session

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; //mico: treba editirat gdje se user salje 
        options.LogoutPath = "/User/Logout";
        options.AccessDeniedPath = "/User/Forbidden";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    }); ;//mico:za cookies

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();//mico: autentikacija
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();