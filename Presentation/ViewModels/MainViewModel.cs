using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.UiImplementation.Commands;

namespace VisualWorkflowBuilder.UiImplementation.ViewModels;

public class MainViewModel
{
    private IWorkflowManager manager;

    
    public ObservableCollection<Job> Jobs { get; } = new();
    
    public ICommand AddJobToWorkspaceCommand {get; set;}
    public ICommand ShowEditWindowCommand {get; set;}
    public MainViewModel(IWorkflowManager workflowManager)
    {
        manager = workflowManager;
        
        
        AddJobToWorkspaceCommand = new RelayCommand(AddJobToWorkspace, CanAddJobToWorkspace);
        ShowEditWindowCommand = new RelayCommand(ShowEditWindow, CanShowEditWindow);
    }

    private bool CanAddJobToWorkspace(object parameter)
    {
        return true;
    }

    private void AddJobToWorkspace(object parameter)
    { 
        Jobs.Add(new Job(){Name = "New Job"});
        Console.WriteLine($"New Job: {Jobs.Count}");
        
    }

    private bool CanShowEditWindow(object parameter)
    {
        return true;
    }

    private void ShowEditWindow(object parameter)
    {
        Console.WriteLine(parameter.ToString());
        EditJobWindow editWindow = new EditJobWindow();
        editWindow.Owner = parameter as MainWindow;
        editWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        editWindow.Show();
    }
    
    
}