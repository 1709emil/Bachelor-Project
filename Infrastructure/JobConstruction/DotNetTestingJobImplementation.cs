using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Infrastructure.JobConstruction
{
    public class DotNetTestingJobImplementation : ITestingJobConstructor
    {
        public Job ConstructTestingJobNoParameters()
        {
            Job job = new Job
            {
                Name = "Testing Job",
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
                        Name = "Set up .NET",
                        Uses = "actions/setup-dotnet@v1",
                        With = new Dictionary<string, string>
                        {
                            { "dotnet-version", "9.0.x" }
                        }
                    },
                    new Step
                    {
                        Name = "Restore dependencies",
                        Run = "dotnet restore"
                    },
                    new Step
                    {
                        Name = "Run tests",
                        Run = "dotnet test --no-build --verbosity normal"
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
                        Name = "Set up .NET",
                        Uses = "actions/setup-dotnet@v1",
                        With = new Dictionary<string, string>
                        {
                            { "dotnet-version", "9.0.x" }
                        }
                    },
                    new Step
                    {
                        Name = "Restore dependencies",
                        Run = "dotnet restore"
                    },
                    new Step
                    {
                        Name = "Run tests",
                        Run = "dotnet test --no-build --verbosity normal"
                    }
                }
            };
            return job;
        }
    }
}
