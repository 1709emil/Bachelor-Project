using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports
{
    public interface IDelpoyJobConstructor
    {
        public Job ConstructDeployJobNoParameters();
        public Job ConstructDeployJobWithName(string Name);
        public Job ConstructDeployJobWithFullParameters(string name, string runsOn, Step steps);
    }
}
