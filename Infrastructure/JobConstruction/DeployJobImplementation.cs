using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Infrastructure.JobConstruction
{
    public class DeployJobImplementation : IDelpoyJobConstructor
    {
        public Job ConstructDeployJobNoParameters()
        {
            Job Job = new Job
            {
                Name = "Deploy Job",
                RunsOn = "ubuntu-latest",
                Steps = new List<Step>
                {
                    new Step
                    {
                        Name = "Checkout code",
                        Uses = "actions/checkout@v2"
                    },
                    new Step
                    {
                        Name = "Deploy to Production",
                        Run = "echo Deploying to production server...\n bash deploy.sh"
                    }
                }
            };
            return Job;
        }

        public Job ConstructDeployJobWithFullParameters(string name, string runsOn, Step steps)
        {
            Job Job = new Job
            {
                Name = name,
                RunsOn = runsOn,
                Steps = new List<Step> { steps }
            };
            return Job;
        }

        public Job ConstructDeployJobWithName(string Name)
        {
            Job Job = new Job
            {
                Name = Name,
                RunsOn = "ubuntu-latest",
                Steps = new List<Step>
                {
                    new Step
                    {
                        Name = "Checkout code",
                        Uses = "actions/checkout@v2"
                    },
                    new Step
                    {
                        Name = "Deploy to Production",
                        Run = "echo Deploying to production server...\n bash deploy.sh"
                    }
                }
            };
            return Job;
        }
    }
}
