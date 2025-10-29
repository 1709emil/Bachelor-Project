using VisualWorkflowBuilder.Abstractions.StepAbstraction;
using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.StepImplement;

public class Stepbuilder : IStep
{
    public Step CreateStep(string actionName, int id)
    {
        Step step = new Step();
        step.ActionName = actionName;
        step.Id = id;
        return step;
      
    }
}