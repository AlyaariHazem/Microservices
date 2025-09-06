using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.Infrastracure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices
                    (this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database");
            //Add Service to the container
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(connectionString);

            // Add infrastructure services here, e.g., DbContext, Repositories, External Services, etc.
            return services;
        }
    }
}
