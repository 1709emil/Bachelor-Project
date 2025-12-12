using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Infrastructure.JobConstruction;
using VisualWorkflowBuilder.Infrastructure.WorkflowConstructor;
using VisualWorkflowBuilder.Infrastructure.YamlTranslator;

namespace VisualWorkflowBuilder.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IObjectToYamlTranslator, ObjectToYamlImplementation>();
        services.AddTransient<IWorkFlowConstructor, WorkFlowConstructorImplementation>();
        services.AddTransient<IBuildJobConstructor, MavenBuildJobImplementation>();
        services.AddTransient<IBuildJobConstructor, DotNetBuildJobImplementation>();
        services.AddTransient<ILintingJobConstructor, LintingJobImplementation>();
        services.AddTransient<ITestingJobConstructor, DotNetTestingJobImplementation>();
        services.AddTransient<ITestingJobConstructor, JUnitTestingJobImplementation>();
        services.AddTransient<IDelpoyJobConstructor, DeployJobImplementation>();
        return services;
    }
}