using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VisualWorkflowBuilder.Application;
using VisualWorkflowBuilder.Infrastructure;


namespace VisualWorkflowBuilder;

public partial class App 
{
    
    private IHost _host = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddApplication();
                services.AddInfrastructure();
                services.AddPresentation();
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