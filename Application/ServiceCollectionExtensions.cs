using Microsoft.Extensions.DependencyInjection;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Application.UseCases;

namespace VisualWorkflowBuilder.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<ObjectToYamlService>();
        return services;
    }
}