using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using SD.TestApi.Application.Interfaces;
using SD.TestApi.Application.Services;

[assembly: InternalsVisibleTo("SD.TestApi.Application.Tests")]

namespace SD.TestApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddSingleton<IImageService, ImageService>();

        return services;
    }
}
