using System.Windows;
using VisualWorkflowBuilder.Abstractions.StepAbstraction;
using VisualWorkflowBuilder.StepImplement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace VisualWorkflowBuilder;

public partial class App : Application
{
    
    private IHost _host = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                
                services.AddSingleton<IStep, Stepbuilder>();
                

              
                
                services.AddTransient<MainWindow>();
            })
            .Build();

        _host.Start();
        var window = _host.Services.GetRequiredService<MainWindow>();
        window.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host.Dispose();
        base.OnExit(e);
    }
}