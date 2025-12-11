using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Infrastructure.WorkflowConstructor
{
    public class MavenBuildJobImplementation : IBuildJobConstructor
    {
        public Job ConstructBuildJobNoParameters()
        {
           Job job = new Job
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
            return job;
        }

        public Job ConstructBuildJobWithFullParameters(string name, string runsOn, Step steps)
        {
           Job job = new Job
            {
                Name = name,
                RunsOn = runsOn,
                Steps = new List<Step> { steps }
            };
            return job;
        }

        public Job ConstructBuildJobWithName(string name)
        {
            Job job = new Job
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
            return job;
        }
    }
}
