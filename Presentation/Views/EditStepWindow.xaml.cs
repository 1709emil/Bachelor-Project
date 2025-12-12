using System.Windows;

namespace VisualWorkflowBuilder.Presentation.Views
{
    public partial class EditStepWindow : Window
    {
        public EditStepWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Maximized;
            Topmost = true;

          
        }
    }
}
