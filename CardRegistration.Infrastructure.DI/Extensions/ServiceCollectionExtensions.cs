using AutoMapper;
using CardRegistration.Application.Impl.Services;
using CardRegistration.Application.Impl.Validators;
using CardRegistration.Application.Profiles;
using CardRegistration.Application.Services;
using CardRegistration.Application.Validators;
using CardRegistration.Domain.Contracts.Repositories;
using CardRegistration.Domain.Contracts.Services;
using CardRegistration.Domain.Services;
using CardRegistration.Infrastructure.DbContexts;
using CardRegistration.Infrastructure.InMemory.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CardRegistration.Infrastructure.DI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            RegisterContext(services);
            RegisterMappers(services);
            RegisterService(services);
            RegisterDomainService(services);
            RegisterValidators(services);
            RegisterRepositories(services);

            return services;
        }

        public static void RegisterContext(IServiceCollection services)
        {
            services.AddScoped<CustomerCardContext>();
        }

        public static void RegisterService(IServiceCollection services)
        {
            services.AddScoped<ICardService, CardService>();
        }

        public static void RegisterDomainService(IServiceCollection services)
        {
            services.AddSingleton<ITokenService, TokenService>();
        }

        public static void RegisterValidators(IServiceCollection services)
        {
            services.AddSingleton<ICardValidator, CardValidator>();
        }

        public static void RegisterMappers(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CardProfile());
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }

        public static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<ICardRepository, CardInMemoryRepository>();
        }
    }
}
