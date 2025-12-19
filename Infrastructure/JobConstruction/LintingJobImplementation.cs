using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Infrastructure.JobConstruction
{
    public class LintingJobImplementation : ILintingJobConstructor
    {
        public Job ConstructLintingJobNoParameters()
        {
            Job job = new Job
            {
                Name = "Linting-Job",
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
                        Name = "Set up DotNet",
                        Uses = "actions/setup-dotnet@v4",
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
                        Name = "Run linter",
                        Run = "dotnet build --no-restore --configuration Release /p:EnforceCodeStyleInBuild=true /p:TreatWarningsAsErrors=false"
                    },
                    new Step
                    {
                        Name = "Run format check",
                        Run = "dotnet format --verify-no-changes --verbosity diagnostic"
                    }
                }
            };

            return job;
        }

        public Job ConstructLintingJobWithFullParameters(string name, string runsOn, Step steps)
        {
            Job job = new Job
            {
                Name = name,
                RunsOn = runsOn,
                Steps = new List<Step> { steps }
            };
            return job;
        }

        public Job ConstructLintingJobWithName(string Name)
        {
            Job job = new Job
            {
                Name = Name,
                RunsOn = "windows-latest",
                Steps = new List<Step>
                {
                    new Step
                    {
                        Name = "Checkout code",
                        Uses = "actions/checkout@v2"
                    },
                    new Step
                    {
                        Name = "Set up DotNet",
                        Uses = "actions/setup-dotnet@v4",
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
                        Name = "Run linter",
                        Run = "dotnet build --no-restore --configuration Release /p:EnforceCodeStyleInBuild=true /p:TreatWarningsAsErrors=false"
                    },
                    new Step
                    {
                        Name = "Run format check",
                        Run = "dotnet format --verify-no-changes --verbosity diagnostic"
                    }
                }
            };
            return job;
        }
    }
}
