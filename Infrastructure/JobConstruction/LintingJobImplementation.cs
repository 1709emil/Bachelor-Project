using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Name = "Linting Job",
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
                        Name = "Set up Node.js",
                        Uses = "actions/setup-node@v2",
                        With = new Dictionary<string, string>
                        {
                            { "node-version", "14" }
                        }
                    },
                    new Step
                    {
                        Name = "Install dependencies",
                        Run = "npm install"
                    },
                    new Step
                    {
                        Name = "Run linter",
                        Run = "npm run lint"
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
                        Name = "Set up Node.js",
                        Uses = "actions/setup-node@v2",
                        With = new Dictionary<string, string>
                        {
                            { "node-version", "14" }
                        }
                    },
                    new Step
                    {
                        Name = "Install dependencies",
                        Run = "npm install"
                    },
                    new Step
                    {
                        Name = "Run linter",
                        Run = "npm run lint"
                    }
                }
            };
            return job;
        }
    }
}
