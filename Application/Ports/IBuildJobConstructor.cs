using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports
{
    public interface IBuildJobConstructor
    {

        public Job ConstructBuildJobNoParameters();
        public Job ConstructBuildJobWithName(string Name);
        public Job ConstructBuildJobWithFullParameters(string name, string runsOn, Step steps);
    }
}
