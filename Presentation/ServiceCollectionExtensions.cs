using Microsoft.Extensions.DependencyInjection;
using VisualWorkflowBuilder.UiImplementation.ViewModels;

namespace VisualWorkflowBuilder;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
        return services;
    }
}