using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Infrastructure.JobConstruction
{
    public class JUnitTestingJobImplementation : ITestingJobConstructor
    {
        public Job ConstructTestingJobNoParameters()
        {
            Job job = new Job
            {
                Name = "JUnit Testing Job",
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
                            {"distribution", "termurin" },
                            { "java-version", "17" }
                        }
                    },
                    new Step
                    {
                        Name = "Run JUnit tests",
                        Run = "mvn -B test"
                    }
                }
            };
            return job;
        }

        public Job ConstructTestingJobWithFullParameters(string name, string runsOn, Step steps)
        {
            Job job = new Job
            {
                Name = name,
                RunsOn = runsOn,
                Steps = new List<Step> { steps }
            };
            return job;
        }

        public Job ConstructTestingJobWithName(string Name)
        {
            Job job = new Job
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
                        Name = "Set up JDK 11",
                        Uses = "actions/setup-java@v2",
                        With = new Dictionary<string, string>
                        {
                            {"distribution", "termurin" },
                            { "java-version", "17" }
                        }
                    },
                    new Step
                    {
                        Name = "Run JUnit tests",
                        Run = "mvn -B test"
                    }
                }
            };
            return job;
        }
    }
}
