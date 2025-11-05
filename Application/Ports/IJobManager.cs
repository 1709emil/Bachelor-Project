using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports;

public interface IJobManager
{
    public void AddStep(Step job);
    public void RemoveStep(Step job);
    public void EditStep(Step job, Step editStep);
    
}