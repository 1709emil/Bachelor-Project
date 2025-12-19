using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.Infrastructure.WorkflowConstructor;
using Xunit;

namespace Test.UnitTests
{
    public class WorkFlowConstructorTests
    {
        [Fact]
        public void WorkFlowConstructor_ConstructWorkFlowNoParameters_ReturnsEmptyWorkflow()
        {
            // Arrange
            IWorkFlowConstructor constructor = new WorkFlowConstructorImplementation();

            // Act
            Workflow workflow = constructor.ConstructWorkFlowNoParameters();

            // Assert
            Assert.NotNull(workflow);
            Assert.Null(workflow.Name);
            Assert.Null(workflow.On);
            Assert.NotNull(workflow.Jobs);
            Assert.Empty(workflow.Jobs);
        }

        [Fact]
        public void WorkFlowConstructor_ConstructWorkFlowWithParameters_SetsAllProperties()
        {
            // Arrange
            IWorkFlowConstructor constructor = new WorkFlowConstructorImplementation();
            string name = "CI/CD Pipeline";
            Triggers triggers = new Triggers
            {
                Push = new BranchTrigger { Branches = new string[] { "main" } }
            };
            Dictionary<string, Job> jobs = new Dictionary<string, Job>
            {
                { "build", new Job { Name = "build", RunsOn = "ubuntu-latest" } }
            };

            // Act
            Workflow workflow = constructor.ConstructWorkFlowWithParameters(name, triggers, jobs);

            // Assert
            Assert.NotNull(workflow);
            Assert.Equal(name, workflow.Name);
            Assert.Equal(triggers, workflow.On);
            Assert.Equal(jobs, workflow.Jobs);
        }

        [Fact]
        public void WorkFlowConstructor_ConstructWorkFlowWithParameters_HandlesMultipleJobs()
        {
            // Arrange
            IWorkFlowConstructor constructor = new WorkFlowConstructorImplementation();
            Dictionary<string, Job> jobs = new Dictionary<string, Job>
            {
                { "build", new Job { Name = "build" } },
                { "test", new Job { Name = "test" } },
                { "deploy", new Job { Name = "deploy" } }
            };

            // Act
            Workflow workflow = constructor.ConstructWorkFlowWithParameters("Pipeline", new Triggers(), jobs);

            // Assert
            Assert.Equal(3, workflow.Jobs!.Count);
            Assert.True(workflow.Jobs.ContainsKey("build"));
            Assert.True(workflow.Jobs.ContainsKey("test"));
            Assert.True(workflow.Jobs.ContainsKey("deploy"));
        }

        [Fact]
        public void WorkFlowConstructor_ConstructWorkFlowWithParameters_HandlesEmptyJobs()
        {
            // Arrange
            IWorkFlowConstructor constructor = new WorkFlowConstructorImplementation();
            Dictionary<string, Job> emptyJobs = new Dictionary<string, Job>();

            // Act
            Workflow workflow = constructor.ConstructWorkFlowWithParameters("Empty", new Triggers(), emptyJobs);

            // Assert
            Assert.NotNull(workflow.Jobs);
            Assert.Empty(workflow.Jobs);
        }

        [Theory]
        [InlineData("CI Pipeline")]
        [InlineData("Deployment Workflow")]
        [InlineData("Testing Suite")]
        public void WorkFlowConstructor_ConstructWorkFlowWithParameters_AcceptsDifferentNames(string workflowName)
        {
            // Arrange
            IWorkFlowConstructor constructor = new WorkFlowConstructorImplementation();

            // Act
            Workflow workflow = constructor.ConstructWorkFlowWithParameters(
                workflowName,
                new Triggers(),
                new Dictionary<string, Job>());

            // Assert
            Assert.Equal(workflowName, workflow.Name);
        }
    }
}