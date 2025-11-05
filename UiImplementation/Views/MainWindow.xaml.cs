using System.Windows;
using VisualWorkflowBuilder.UiImplementation.ViewModels;

namespace VisualWorkflowBuilder;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}