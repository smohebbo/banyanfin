using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Banyan.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Banyan.IOC;

public static class ServiceInstance
{
public static void RegisterMCCCULServiceInstance(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(MCCCULContext));
            services.AddDbContextPool<MCCCULContext>(options => options.UseSqlServer(connectionString));
    }
}
