using Application.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services,IConfiguration configuration)
        {
            // Register your database context and other related services here
            // Example:
            // services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseSqlServer(connectionString));


            return services
                .AddDbContext<ApplicationDbContext>(options => 
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Register your repositories here
            // Example:
            // services.AddScoped<IYourRepository, YourRepositoryImplementation>();
            return services
                .AddTransient(typeof(IReadRepositoryAsync<,>), typeof(ReadRepositoryAsync<,>))
                .AddTransient(typeof(IWriteRepositoryAsync<,>), typeof(WriteRepositoryAsync<,>))
                .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        }
    }
}
