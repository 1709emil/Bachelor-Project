using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.Abstractions.StepAbstraction;

public interface IStep
{  
    Step CreateStep(string actionName, int id);
}