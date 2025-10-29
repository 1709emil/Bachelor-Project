using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.Abstractions;

public interface IStep
{  
    Step CreateStep(string actionName, int id);
}