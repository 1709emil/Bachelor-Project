using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VisualWorkflowBuilder.Abstractions;
using VisualWorkflowBuilder.Core;
using VisualWorkflowBuilder.YamlTranslator;

namespace VisualWorkflowBuilder;

public partial class App 
{
    
    private IHost _host = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IObjectToYamlTranslator, ObjectToYamlImplementation>();
                services.AddTransient<ObjectToYamlService>();
                services.AddTransient<MainWindow>();
            })
            .Build();

        _host.Start();
        var window = _host.Services.GetRequiredService<MainWindow>();
        window.Show();

        base.OnStartup(e);

        ObjectToYamlService test =_host.Services.GetRequiredService<ObjectToYamlService>();
        test.test();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host.Dispose();
        base.OnExit(e);
    }
}