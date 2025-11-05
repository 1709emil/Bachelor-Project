using System.Collections.ObjectModel;
using VisualWorkflowBuilder.Abstractions;
using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.WorkflowManager;

public class WorkflowManagerImplementation : IWorkflowManager
{
    private Workflow Workflow { get; } = new()
    {
        Name = "test workflow",
    };
    

    public  Workflow GetWorkflow()
    {
        return Workflow;
    }

    public void AddJobToWorkflow(Job job)
    {
        
    }

    public void RemoveJobFromWorkflow(Job job)
    {
        
    }

    public void EditJob(Job job,Job editJob)
    {
        
    }
}