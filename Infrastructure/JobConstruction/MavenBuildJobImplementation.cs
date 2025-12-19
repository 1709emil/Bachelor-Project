using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Infrastructure.JobConstruction
{
    public class MavenBuildJobImplementation : IBuildJobConstructor
    {
        public Job ConstructBuildJobNoParameters()
        {
            Job Job = new Job
            {
                Name = "Maven Build Job",
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
                        Name = "Set up JDK 11",
                        Uses = "actions/setup-java@v2",
                        With = new Dictionary<string, string>
                        {
                            { "java-version", "11" }
                        }
                    },
                    new Step
                    {
                        Name = "Build with Maven",
                        Run = "mvn clean install"
                    }
                }
            };
            return Job;
        }

        public Job ConstructBuildJobWithFullParameters(string name, string runsOn, Step steps)
        {
            Job Job = new Job
            {
                Name = name,
                RunsOn = runsOn,
                Steps = new List<Step> { steps }
            };
            return Job;
        }

        public Job ConstructBuildJobWithName(string name)
        {
            Job Job = new Job
            {
                Name = name,
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
                        Name = "Set up JDK 11",
                        Uses = "actions/setup-java@v2",
                        With = new Dictionary<string, string>
                        {
                            { "java-version", "11" }
                        }
                    },
                    new Step
                    {
                        Name = "Build with Maven",
                        Run = "mvn clean install"
                    }
                }
            };
            return Job;
        }
    }
}
