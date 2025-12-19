using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.Infrastructure.JobConstruction;
using VisualWorkflowBuilder.Infrastructure.WorkflowConstructor;
using Xunit;

namespace Test.IntegrationTests
{
    public class WorkflowConstructionIntegrationTests
    {
        [Fact]
        public void CompleteWorkflow_WithMultipleJobTypes_BuildsSuccessfully()
        {
            // Arrange
            IWorkFlowConstructor workflowConstructor = new WorkFlowConstructorImplementation();
            IBuildJobConstructor buildConstructor = new DotNetBuildJobImplementation();
            ITestingJobConstructor testConstructor = new DotNetTestingJobImplementation();
            IDelpoyJobConstructor deployConstructor = new DeployJobImplementation();

            Job buildJob = buildConstructor.ConstructBuildJobWithName("Build");
            Job testJob = testConstructor.ConstructTestingJobWithName("Test");
            testJob.Needs = "Build";
            Job deployJob = deployConstructor.ConstructDeployJobWithName("Deploy");
            deployJob.Needs = "Test";

            Dictionary<string, Job> jobs = new Dictionary<string, Job>
            {
                { buildJob.Name, buildJob },
                { testJob.Name, testJob },
                { deployJob.Name, deployJob }
            };

            Triggers triggers = new Triggers
            {
                Push = new BranchTrigger { Branches = new string[] { "main" } }
            };

            // Act
            Workflow workflow = workflowConstructor.ConstructWorkFlowWithParameters("CI/CD Pipeline", triggers, jobs);

            // Assert
            Assert.NotNull(workflow);
            Assert.Equal("CI/CD Pipeline", workflow.Name);
            Assert.Equal(3, workflow.Jobs!.Count);
            Assert.Equal("Build", workflow.Jobs["Test"].Needs);
            Assert.Equal("Test", workflow.Jobs["Deploy"].Needs);
        }

        [Fact]
        public void WorkflowWithDifferentBuildSystems_Maven_And_DotNet_CoexistSuccessfully()
        {
            // Arrange
            IWorkFlowConstructor workflowConstructor = new WorkFlowConstructorImplementation();
            IBuildJobConstructor mavenConstructor = new MavenBuildJobImplementation();
            IBuildJobConstructor dotnetConstructor = new DotNetBuildJobImplementation();

            Job mavenJob = mavenConstructor.ConstructBuildJobWithName("Build Java");
            Job dotnetJob = dotnetConstructor.ConstructBuildJobWithName("Build .NET");

            Dictionary<string, Job> jobs = new Dictionary<string, Job>
            {
                { mavenJob.Name, mavenJob },
                { dotnetJob.Name, dotnetJob }
            };

            // Act
            Workflow workflow = workflowConstructor.ConstructWorkFlowWithParameters(
                "Multi-Platform Build",
                new Triggers(),
                jobs);

            // Assert
            Assert.Equal(2, workflow.Jobs!.Count);
            Assert.True(workflow.Jobs.ContainsKey("Build Java"));
            Assert.True(workflow.Jobs.ContainsKey("Build .NET"));
        }

        [Fact]
        public void WorkflowWithLinting_Build_Test_Deploy_Pipeline_ExecutesInOrder()
        {
            // Arrange
            IWorkFlowConstructor workflowConstructor = new WorkFlowConstructorImplementation();
            ILintingJobConstructor lintConstructor = new LintingJobImplementation();
            IBuildJobConstructor buildConstructor = new DotNetBuildJobImplementation();
            ITestingJobConstructor testConstructor = new DotNetTestingJobImplementation();
            IDelpoyJobConstructor deployConstructor = new DeployJobImplementation();

            Job lintJob = lintConstructor.ConstructLintingJobWithName("Lint");
            Job buildJob = buildConstructor.ConstructBuildJobWithName("Build");
            buildJob.Needs = "Lint";
            Job testJob = testConstructor.ConstructTestingJobWithName("Test");
            testJob.Needs = "Build";
            Job deployJob = deployConstructor.ConstructDeployJobWithName("Deploy");
            deployJob.Needs = "Test";

            Dictionary<string, Job> jobs = new Dictionary<string, Job>
            {
                { lintJob.Name, lintJob },
                { buildJob.Name, buildJob },
                { testJob.Name, testJob },
                { deployJob.Name, deployJob }
            };

            // Act
            Workflow workflow = workflowConstructor.ConstructWorkFlowWithParameters(
                "Full Pipeline",
                new Triggers { Push = new BranchTrigger { Branches = new string[] { "main" } } },
                jobs);

            // Assert
            Assert.Equal(4, workflow.Jobs!.Count);
            Assert.Equal("Lint", workflow.Jobs["Build"].Needs);
            Assert.Equal("Build", workflow.Jobs["Test"].Needs);
            Assert.Equal("Test", workflow.Jobs["Deploy"].Needs);
        }

        [Fact]
        public void JobConstructors_AllImplementations_CreateCompatibleJobs()
        {
            // Arrange
            List<IBuildJobConstructor> buildConstructors = new List<IBuildJobConstructor>
            {
                new DotNetBuildJobImplementation(),
                new MavenBuildJobImplementation()
            };

            List<ITestingJobConstructor> testConstructors = new List<ITestingJobConstructor>
            {
                new DotNetTestingJobImplementation(),
                new JUnitTestingJobImplementation()
            };

            // Act & Assert
            foreach (IBuildJobConstructor constructor in buildConstructors)
            {
                Job job = constructor.ConstructBuildJobNoParameters();
                Assert.NotNull(job);
                Assert.NotNull(job.Name);
                Assert.NotNull(job.RunsOn);
                Assert.NotNull(job.Steps);
                Assert.NotEmpty(job.Steps);
            }

            foreach (ITestingJobConstructor constructor in testConstructors)
            {
                Job job = constructor.ConstructTestingJobNoParameters();
                Assert.NotNull(job);
                Assert.NotNull(job.Name);
                Assert.NotNull(job.RunsOn);
                Assert.NotNull(job.Steps);
                Assert.NotEmpty(job.Steps);
            }
        }

        [Fact]
        public void WorkflowWithCustomSteps_AcceptsEnvironmentVariables()
        {
            // Arrange
            IBuildJobConstructor constructor = new DotNetBuildJobImplementation();
            Step customStep = new Step
            {
                Name = "Build with env",
                Run = "dotnet build",
                Env = new Dictionary<string, string>
                {
                    { "CONFIGURATION", "Release" },
                    { "VERSION", "1.0.0" }
                }
            };

            // Act
            Job job = constructor.ConstructBuildJobWithFullParameters("Custom Build", "ubuntu-latest", customStep);

            // Assert
            Assert.NotNull(job.Steps);
            Assert.Single(job.Steps);
            Assert.NotNull(job.Steps[0].Env);
            Assert.Equal(2, job.Steps[0].Env.Count);
            Assert.Equal("Release", job.Steps[0].Env["CONFIGURATION"]);
        }
    }
}