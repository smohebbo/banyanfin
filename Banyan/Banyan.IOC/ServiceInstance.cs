using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Banyan.Data.Models;
using Banyan.Repository.Models;
using Banyan.Service;

namespace Banyan.IOC;

public static class ServiceInstance
{
public static void RegisterMCCCULServiceInstance(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("devmohebbo")
            ?? throw new InvalidOperationException("The BanyanFin database connection string is not configured.");
        services.AddDbContextPool<BanyanFinContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions => sqlOptions.UseNetTopologySuite()));
        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }
}
