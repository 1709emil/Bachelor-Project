using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.Abstractions;

public interface IJobManager
{
    public void AddStep(Step job);
    public void RemoveStep(Step job);
    public void EditStep(Step job, Step editStep);
    
}