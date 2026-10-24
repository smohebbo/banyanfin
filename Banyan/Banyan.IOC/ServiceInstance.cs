using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Banyan.Data.Models;

namespace Banyan.IOC;

public static class ServiceInstance
{
public static void RegisterMCCCULServiceInstance(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(BanyanFinContext));
            services.AddDbContextPool<BanyanFinContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions => sqlOptions.UseNetTopologySuite()));
    }
}
