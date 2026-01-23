using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class Startup
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register application services, repositories, MediatR handlers, etc.
            // services.AddMediatR(typeof(Startup).Assembly);
            // services.AddScoped<IUnitOfWork<int>, UnitOfWork<int>>();
            var assembly = Assembly.GetExecutingAssembly();

            return services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });
        }
    }
}
