using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports;

public interface IWorkflowManager
{
    public Workflow GetWorkflow();


    public void AddJobToWorkflow(Job job);
    public void RemoveJobFromWorkflow(Job job);
    public void EditJob(Job job,Job editJob);
    

}