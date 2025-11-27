using System.Windows;
using VisualWorkflowBuilder.UiImplementation.ViewModels;

namespace VisualWorkflowBuilder;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Maximized;

    }
}