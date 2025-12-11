using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Infrastructure.JobConstruction
{
    public class DotNetBuildJobImplementation : IBuildJobConstructor
    {
        public Job ConstructBuildJobNoParameters()
        {
            Job Job = new Job
            {
                Name = "Build",
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
                        Name = "Build",
                        Run = "dotnet build --no-restore --configuration Release"
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

        public Job ConstructBuildJobWithName(string Name)
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
                        Name = "Build",
                        Run = "dotnet build --no-restore --configuration Release"
                    }
                }
            };
            return job;
        }
    }
}
