using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.Presentation.ViewModels;
using VisualWorkflowBuilder.UiImplementation.ViewModels;
using VisualWorkflowBuilder.UiImplementation.Commands;

namespace VisualWorkflowBuilder.UiImplementation.ViewModels;

public class MainViewModel
{
    private readonly IWorkflowManager _manager;

    public ObservableCollection<JobNodeViewModel> Nodes { get; } = new();

    public ICommand AddJobToWorkspaceCommand { get; }
    public ICommand ShowEditWindowCommand { get; }

    private const double LeftMargin = 20.0;
    private const double TopMargin = 20.0;
    private const double HorizontalSpacing = 180.0; 
    private const double VerticalSpacing = 40.0;
    private const double DefaultNodeWidth = 160.0;
    private const double DefaultNodeHeight = 100.0;

    public MainViewModel(IWorkflowManager workflowManager)
    {
        _manager = workflowManager;

        AddJobToWorkspaceCommand = new RelayCommand(AddJobToWorkspace, _ => true);
        ShowEditWindowCommand = new RelayCommand(ShowEditWindow, _ => true);
    }

    private void AddJobToWorkspace(object? parameter)
    {
        var job = new Job() { Name = $"Job {Nodes.Count + 1}" };
        var node = new JobNodeViewModel(job);

        var topNodes = Nodes.Where(n => string.IsNullOrWhiteSpace(n.Job.Needs)
                                        || !Nodes.Any(x => x.Job.Name == n.Job.Needs)).ToList();

        node.CanvasLeft = LeftMargin + topNodes.Count * HorizontalSpacing;
        node.CanvasTop = TopMargin;

        Nodes.Add(node);

        RecalculateLayout();
    }

    private void ShowEditWindow(object? parameter)
    {
        if (parameter is not JobNodeViewModel nodeVm) return;
        var originalNeeds = nodeVm.Job.Needs;
        var originalName = nodeVm.Job.Name;

        var editWindow = new EditJobWindow
        {
            Owner = System.Windows.Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            DataContext = new EditJobViewModel(nodeVm.Job)
        };

        editWindow.ShowDialog();

        if (originalNeeds != nodeVm.Job.Needs || originalName != nodeVm.Job.Name)
        {
            RecalculateLayout();
        }
        else
        {
           
        }
    }

    // Public helper to recalc positions; can be called from UI as well.
    public void RecalculateLayout()
    {
        var nodeByName = Nodes.ToDictionary(n => n.Job.Name);

        var topLevel = Nodes
            .Where(n => string.IsNullOrWhiteSpace(n.Job.Needs) || !nodeByName.ContainsKey(n.Job.Needs))
            .ToList();

        for (int i = 0; i < topLevel.Count; i++)
        {
            var n = topLevel[i];
            n.CanvasLeft = LeftMargin + i * HorizontalSpacing;
            n.CanvasTop = TopMargin;
        }

        var handled = new HashSet<JobNodeViewModel>(topLevel);
        var queue = new Queue<JobNodeViewModel>(topLevel);

        while (queue.Count > 0)
        {
            var parent = queue.Dequeue();

           
            var children = Nodes.Where(n => n != parent && n.Job.Needs == parent.Job.Name).ToList();
            for (int ci = 0; ci < children.Count; ci++)
            {
                var child = children[ci];
               
                child.CanvasLeft = parent.CanvasLeft + ci * (DefaultNodeWidth + 8) * 0.0; 
                child.CanvasTop = parent.CanvasTop + DefaultNodeHeight + VerticalSpacing + ci * (DefaultNodeHeight + 8);
                if (!handled.Contains(child))
                {
                    handled.Add(child);
                    queue.Enqueue(child);
                }
            }
        }

       
        var remaining = Nodes.Except(handled).ToList();
        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].CanvasLeft = LeftMargin + (topLevel.Count + i) * HorizontalSpacing;
            remaining[i].CanvasTop = TopMargin;
        }
    }
}