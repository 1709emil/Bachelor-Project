using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Infrastructure.WorkflowConstructor
{
    public class WorkFlowConstructorImplementation : IWorkFlowConstructor
    {
        public Workflow ConstructWorkFlowNoParameters()
        {
            Workflow workflow = new Workflow();
            return workflow;
        }

        public Workflow ConstructWorkFlowWithParameters(string name, Triggers triggers, Dictionary<string, Job> jobs)
        {
            Workflow workflow = new Workflow();
            workflow.Name = name;
            workflow.On = triggers;
            workflow.Jobs = jobs;

            return workflow;
        }
    }
}
