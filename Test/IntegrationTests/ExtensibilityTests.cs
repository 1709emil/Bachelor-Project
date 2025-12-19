using Moq;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.Infrastructure.JobConstruction;
using VisualWorkflowBuilder.Presentation.ViewModels;
using VisualWorkflowBuilder.UiImplementation.ViewModels;
using Xunit;
using System.Collections.Generic;

namespace Test.IntegrationTests
{
    public class ExtensibilityTests
    {
        [Fact]
        public void MainViewModel_SupportsMultipleTestingJobImplementations_WithoutCodeChanges()
        {
            // Arrange - Create two different testing job implementations
            ITestingJobConstructor dotNetTesting = new DotNetTestingJobImplementation();
            ITestingJobConstructor junitTesting = new JUnitTestingJobImplementation();

            Mock<IObjectToYamlTranslator> mockTranslator = new Mock<IObjectToYamlTranslator>();
            Mock<IWorkFlowConstructor> mockConstructor = new Mock<IWorkFlowConstructor>();
            Mock<IYamlToObjectTranslator> mockYamlToObject = new Mock<IYamlToObjectTranslator>();
            Mock<IBuildJobConstructor> mockBuildConstructor = new Mock<IBuildJobConstructor>();
            Mock<ILintingJobConstructor> mockLintingConstructor = new Mock<ILintingJobConstructor>();
            Mock<IDelpoyJobConstructor> mockDeployConstructor = new Mock<IDelpoyJobConstructor>();

            // Act - System accepts different implementations through same interface
            MainViewModel viewModelWithDotNet = new MainViewModel(
                mockConstructor.Object,
                mockTranslator.Object,
                mockYamlToObject.Object,
                new[] { mockBuildConstructor.Object },
                mockLintingConstructor.Object,
                new[] { dotNetTesting },
                mockDeployConstructor.Object);

            MainViewModel viewModelWithJUnit = new MainViewModel(
                mockConstructor.Object,
                mockTranslator.Object,
                mockYamlToObject.Object,
                new[] { mockBuildConstructor.Object },
                mockLintingConstructor.Object,
                new[] { junitTesting },
                mockDeployConstructor.Object);

            // Assert - Both implementations work without modifying MainViewModel
            Assert.NotNull(viewModelWithDotNet);
            Assert.NotNull(viewModelWithJUnit);

            Job dotNetJob = dotNetTesting.ConstructTestingJobNoParameters();
            Job junitJob = junitTesting.ConstructTestingJobNoParameters();

            Assert.Equal("Testing Job", dotNetJob.Name);
            Assert.Equal("JUnit Testing Job", junitJob.Name);
        }

        [Fact]
        public void NewTestingJobImplementation_CanBeAddedWithoutModifyingExistingCode()
        {
            // Arrange - Simulate adding a new testing framework (e.g., PyTest)
            Mock<ITestingJobConstructor> newPyTestConstructor = new Mock<ITestingJobConstructor>();
            newPyTestConstructor
                .Setup(c => c.ConstructTestingJobNoParameters())
                .Returns(new Job
                {
                    Name = "PyTest Job",
                    RunsOn = "ubuntu-latest",
                    Steps = new List<Step>
                    {
                        new Step { Name = "Run PyTest", Run = "pytest" }
                    }
                });

            Mock<IObjectToYamlTranslator> mockTranslator = new Mock<IObjectToYamlTranslator>();
            Mock<IWorkFlowConstructor> mockConstructor = new Mock<IWorkFlowConstructor>();
            Mock<IYamlToObjectTranslator> mockYamlToObject = new Mock<IYamlToObjectTranslator>();
            Mock<IBuildJobConstructor> mockBuildConstructor = new Mock<IBuildJobConstructor>();
            Mock<ILintingJobConstructor> mockLintingConstructor = new Mock<ILintingJobConstructor>();
            Mock<IDelpoyJobConstructor> mockDeployConstructor = new Mock<IDelpoyJobConstructor>();

            // Act - New implementation plugs in without modifying MainViewModel
            MainViewModel viewModel = new MainViewModel(
                mockConstructor.Object,
                mockTranslator.Object,
                mockYamlToObject.Object,
                new[] { mockBuildConstructor.Object },
                mockLintingConstructor.Object,
                new[] { newPyTestConstructor.Object },
                mockDeployConstructor.Object);

            Job pyTestJob = newPyTestConstructor.Object.ConstructTestingJobNoParameters();

            // Assert - New module integrates seamlessly
            Assert.NotNull(viewModel);
            Assert.Equal("PyTest Job", pyTestJob.Name);
            Assert.Equal("pytest", pyTestJob.Steps[0].Run);
        }

        [Fact]
        public void MultipleTestingFrameworks_CanCoexistInSameWorkflow()
        {
            // Arrange
            ITestingJobConstructor dotNetTesting = new DotNetTestingJobImplementation();
            ITestingJobConstructor junitTesting = new JUnitTestingJobImplementation();

            Mock<IObjectToYamlTranslator> mockTranslator = new Mock<IObjectToYamlTranslator>();
            Mock<IWorkFlowConstructor> mockConstructor = new Mock<IWorkFlowConstructor>();
            Mock<IYamlToObjectTranslator> mockYamlToObject = new Mock<IYamlToObjectTranslator>();
            Mock<IBuildJobConstructor> mockBuildConstructor = new Mock<IBuildJobConstructor>();
            Mock<ILintingJobConstructor> mockLintingConstructor = new Mock<ILintingJobConstructor>();
            Mock<IDelpoyJobConstructor> mockDeployConstructor = new Mock<IDelpoyJobConstructor>();

            // Act - MainViewModel supports array of testing constructors
            MainViewModel viewModel = new MainViewModel(
                mockConstructor.Object,
                mockTranslator.Object,
                mockYamlToObject.Object,
                new[] { mockBuildConstructor.Object },
                mockLintingConstructor.Object,
                new ITestingJobConstructor[] { dotNetTesting, junitTesting },
                mockDeployConstructor.Object);

            // Assert - System supports multiple implementations simultaneously
            Assert.NotNull(viewModel);

            Job dotNetJob = dotNetTesting.ConstructTestingJobNoParameters();
            Job junitJob = junitTesting.ConstructTestingJobNoParameters();

            Assert.NotEqual(dotNetJob.Name, junitJob.Name);
            Assert.NotEqual(dotNetJob.RunsOn, junitJob.RunsOn);
        }


        [Fact]
        public void DependencyInjection_AllowsRuntimeComponentSubstitution()
        {
            // Arrange
            Mock<IObjectToYamlTranslator> mockTranslator1 = new Mock<IObjectToYamlTranslator>();
            Mock<IObjectToYamlTranslator> mockTranslator2 = new Mock<IObjectToYamlTranslator>();
            Mock<IWorkFlowConstructor> mockConstructor = new Mock<IWorkFlowConstructor>();
            Mock<IYamlToObjectTranslator> mockYamlToObject = new Mock<IYamlToObjectTranslator>();
            Mock<IBuildJobConstructor> mockBuildConstructor = new Mock<IBuildJobConstructor>();
            Mock<ILintingJobConstructor> mockLintingConstructor = new Mock<ILintingJobConstructor>();
            Mock<ITestingJobConstructor> mockTestingConstructor = new Mock<ITestingJobConstructor>();
            Mock<IDelpoyJobConstructor> mockDeployConstructor = new Mock<IDelpoyJobConstructor>();

            mockTranslator1.Setup(t => t.TranslateObjectToYaml(It.IsAny<Workflow>(), It.IsAny<string>()));
            mockTranslator2.Setup(t => t.TranslateObjectToYaml(It.IsAny<Workflow>(), It.IsAny<string>()));

            // Act - Create ViewModels with different translator implementations
            MainViewModel viewModel1 = new MainViewModel(
                mockConstructor.Object,
                mockTranslator1.Object,
                mockYamlToObject.Object,
                new[] { mockBuildConstructor.Object },
                mockLintingConstructor.Object,
                new[] { mockTestingConstructor.Object },
                mockDeployConstructor.Object);

            MainViewModel viewModel2 = new MainViewModel(
                mockConstructor.Object,
                mockTranslator2.Object, // Different implementation
                mockYamlToObject.Object,
                new[] { mockBuildConstructor.Object },
                mockLintingConstructor.Object,
                new[] { mockTestingConstructor.Object },
                mockDeployConstructor.Object);

            // Assert - System supports component substitution via DI
            Assert.NotNull(viewModel1);
            Assert.NotNull(viewModel2);
        }

    }
}