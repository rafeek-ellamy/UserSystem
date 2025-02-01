using Configurations;
using Configurations.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserSystem.Data;
using UserSystem.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddDbContext<UserSystemDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DataSource")));

builder.Services.AddIdentity<UserProfile, IdentityRole>()
    .AddEntityFrameworkStores<UserSystemDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add methods Extensions
builder.Services.AddInjectionApplication(builder.Configuration);
builder.Services.AddLocalization(option => option.ResourcesPath = "");


builder.Services.AddExceptionHandler<GlobalErrorHandling>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<UserSystemDbContext>();
    dataContext.Database.Migrate();
    await dataContext.SeedAsync(dataContext);
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseBuildExtensions();

app.UseAuthorization();
app.MapControllers();
app.Run();