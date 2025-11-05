using System.Collections.ObjectModel;
using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.Abstractions;

public interface IWorkflowManager
{
    public Workflow GetWorkflow();


    public void AddJobToWorkflow(Job job);
    public void RemoveJobFromWorkflow(Job job);
    public void EditJob(Job job,Job editJob);
    

}