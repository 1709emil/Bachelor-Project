using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Infrastructure.WorkflowConstructor;
using VisualWorkflowBuilder.Infrastructure.YamlTranslator;

namespace VisualWorkflowBuilder.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IObjectToYamlTranslator, ObjectToYamlImplementation>();
        services.AddScoped<IWorkFlowConstructor, WorkFlowConstructorImplementation>();
        services.AddScoped<IBuildJobConstructor, MavenBuildJobImplementation>();
        return services;
    }
}