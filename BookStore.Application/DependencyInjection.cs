


using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services) 
        {

            // якщо будуть MediatR, FluentValidation, AutoMapper, CQRS то тут реєструються

            return services;
        }
    }
}
