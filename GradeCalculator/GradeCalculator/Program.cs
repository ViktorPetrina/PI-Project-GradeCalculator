using GradeCalculator.AutoMapper;
using GradeCalculator.Models;
using GradeCalculator.Repository;
using GradeCalculator.Security;
using GradeCalculator.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PiGradeCalculatorContext>(options => {
    options.UseSqlServer("name=ConnectionStrings:connection");
});
builder.Services.AddScoped<StatistikaService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IRepository<Predmet>, PredmetRepository>();
builder.Services.AddScoped<IRepository<Godina> ,GodinaRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();