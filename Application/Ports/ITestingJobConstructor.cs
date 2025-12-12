using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports
{
    public interface ITestingJobConstructor
    {
        public Job ConstructTestingJobNoParameters();
        public Job ConstructTestingJobWithName(string Name);
        public Job ConstructTestingJobWithFullParameters(string name, string runsOn, Step steps);
    }
}
