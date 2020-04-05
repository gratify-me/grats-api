using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gratify.Api.Database
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddGratsDb(this IServiceCollection services, DatabaseSettings settings)
        {
            settings.EnsureValid();
            if (settings.UseInMemory.Value)
            {
                services.AddDbContext<GratsDb>(options => options.UseInMemoryDatabase("Grats.Api.Database.InMemory"));
            }
            else
            {
                services.AddDbContext<GratsDb>(options => options.UseSqlServer(settings.ConnectionString));
            }

            services.AddSingleton(services => settings);
            services.AddTransient<MigrationService>();

            return services;
        }
    }
}
