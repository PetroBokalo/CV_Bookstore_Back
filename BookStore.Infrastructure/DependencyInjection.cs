
using BookStore.Application.Common;
using BookStore.Application.Interfaces;
using BookStore.Infrastructure.Identity;
using BookStore.Infrastructure.Persistence;
using BookStore.Infrastructure.Repositories;
using BookStore.Infrastructure.Services;
using BookStoreAPI.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BookStore.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BookStoreDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"),
                b => b.MigrationsAssembly(typeof(BookStoreDbContext).Assembly.FullName)
                )); // add database context


            services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<BookStoreDbContext>()
            .AddDefaultTokenProviders();


            services.AddScoped<IVerifyTokenRepository, VerifyTokenRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAccountService, AccountService>();


            // Mail config
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            return services;
        }

    }
}
