using BookStoreAPI.Data;
using BookStoreAPI.Repositories.Implementations;
using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services;
using BookStoreAPI.Services.Implementations;
using BookStoreAPI.Services.Interfaces;

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

builder.Services.AddDbContext<BookStoreDbContext>(); // add database context
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<TokenService>();

var app = builder.Build();

app.UseCors("AllowFront");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
