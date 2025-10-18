using BookStoreAPI.Data;
using BookStoreAPI.Entities;
using BookStoreAPI.Models;
using BookStoreAPI.Repositories.Implementations;
using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services;
using BookStoreAPI.Services.Implementations;
using BookStoreAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options => 
{

    options.AddPolicy("AllowFront",
        policy => policy
            .WithOrigins("http://127.0.0.1:5500")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

});

builder.Services.AddDbContext<BookStoreDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"))); // add database context

builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<BookStoreDbContext>()
    .AddDefaultTokenProviders();
    


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVerifyTokenRepository, VerifyTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IResetPasswordTokenRepository, ResetPasswordTokenRepository>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();

// Mail config
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailSettings>();

// Jwt Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))

    };


});


var app = builder.Build();

app.UseCors("AllowFront");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
