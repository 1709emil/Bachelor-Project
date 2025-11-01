using VisualWorkflowBuilder.Abstractions;
using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.StepImplement;

public class test
{
    private readonly IStep myStep;
    
    public test(IStep step)
    {
        myStep = step;
    }

    public void useIStep()
    {
       Step bob = myStep.CreateStep("test of DI", 1);
        Console.WriteLine(bob.ActionName +" : " +bob.Id);
    }
}