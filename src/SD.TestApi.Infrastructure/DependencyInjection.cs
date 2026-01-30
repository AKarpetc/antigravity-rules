using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Infrastructure.External;
using SD.TestApi.Infrastructure.Persistence;

namespace SD.TestApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SettingsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ISettingsRepository, SettingsRepository>();
services.AddScoped<IImageRepository, ImageRepository>();
services.AddScoped<ICartAssistantExternalService, CartAssistantExternalService>();

        return services;
    }
}
