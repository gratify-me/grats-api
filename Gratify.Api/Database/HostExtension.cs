using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gratify.Api.Database
{
    public static class HostExtension
    {
        public static async Task ApplyDatabaseMigrationsAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var migrationService = scope.ServiceProvider.GetRequiredService<MigrationService>();
            await migrationService.ApplyDatabaseMigrationsAsync();
        }
    }
}
