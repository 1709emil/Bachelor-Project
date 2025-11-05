using System.Collections.ObjectModel;
using System.Windows.Input;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.UiImplementation.Commands;

namespace VisualWorkflowBuilder.UiImplementation.ViewModels;

public class MainViewModel
{
    private IWorkflowManager manager;

    public ObservableCollection<Workflow> Workflow { get; } = new();
    
    public ICommand AddJobToWorkspaceCommand {get; set;}
    
    public MainViewModel(IWorkflowManager workflowManager)
    {
        manager = workflowManager;
        Workflow.Add(manager.GetWorkflow());  
        
        AddJobToWorkspaceCommand = new RelayCommand(AddJobToWorkspace, CanAddJobToWorkspace);
    }

    private bool CanAddJobToWorkspace(object parameter)
    {
        return true;
    }

    private void AddJobToWorkspace(object parameter)
    {
        
    }
    
    
}