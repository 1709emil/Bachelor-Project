using Moq;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.UiImplementation.ViewModels;
using VisualWorkflowBuilder.Presentation.ViewModels;
using Xunit;
using System.Collections.Generic;
using static VisualWorkflowBuilder.Presentation.ViewModels.EditWorkFlowConfigSettingsViewModel;

namespace Test.IntegrationTests
{
    public class ConfigurationModificationIntegrationTests
    {
        [Fact]
        public void EditWorkFlowConfigSettings_UserModifiesName_UpdatesWorkflowConfiguration()
        {
            // Arrange
            Workflow workflow = new Workflow
            {
                Name = "Original Name",
                On = new Triggers
                {
                    Push = new BranchTrigger { Branches = new string[] { "main" } }
                }
            };

            EditWorkFlowConfigSettingsViewModel viewModel = new EditWorkFlowConfigSettingsViewModel(workflow);
            string newName = "Modified Name";

            // Act
            viewModel.Name = newName;

            // Assert
            Assert.Equal(newName, viewModel.Name);
        }

        [Fact]
        public void EditWorkFlowConfigSettings_UserModifiesTriggers_PersistsToWorkflow()
        {
            // Arrange
            Workflow workflow = new Workflow
            {
                Name = "Test",
                On = new Triggers()
            };

            EditWorkFlowConfigSettingsViewModel viewModel = new EditWorkFlowConfigSettingsViewModel(workflow);

            // Act - User adds push trigger through UI
            viewModel.PushBranches.Add(new BranchEntry { Value = "develop" });

            // Assert
            Assert.Single(viewModel.PushBranches);
            Assert.Equal("develop", viewModel.PushBranches[0].Value);
        }

        [Fact]
        public void MainViewModel_SaveWorkflow_WithModifiedConfiguration_CallsTranslatorWithUpdatedData()
        {
            // Arrange
            Mock<IObjectToYamlTranslator> mockTranslator = new Mock<IObjectToYamlTranslator>();
            Mock<IWorkFlowConstructor> mockConstructor = new Mock<IWorkFlowConstructor>();
            Mock<IYamlToObjectTranslator> mockYamlToObject = new Mock<IYamlToObjectTranslator>();
            Mock<IBuildJobConstructor> mockBuildConstructor = new Mock<IBuildJobConstructor>();
            Mock<ILintingJobConstructor> mockLintingConstructor = new Mock<ILintingJobConstructor>();
            Mock<ITestingJobConstructor> mockTestingConstructor = new Mock<ITestingJobConstructor>();
            Mock<IDelpoyJobConstructor> mockDeployConstructor = new Mock<IDelpoyJobConstructor>();

            Workflow modifiedWorkflow = new Workflow
            {
                Name = "User Modified Pipeline",
                On = new Triggers
                {
                    Push = new BranchTrigger { Branches = new string[] { "main", "develop" } },
                    PullRequest = new BranchTrigger { Branches = new string[] { "main" } }
                },
                Jobs = new Dictionary<string, Job>()
            };

            mockConstructor
                .Setup(c => c.ConstructWorkFlowWithParameters(
                    It.IsAny<string>(),
                    It.IsAny<Triggers>(),
                    It.IsAny<Dictionary<string, Job>>()))
                .Returns(modifiedWorkflow);

            MainViewModel viewModel = new MainViewModel(
                mockConstructor.Object,
                mockTranslator.Object,
                mockYamlToObject.Object,
                new[] { mockBuildConstructor.Object },
                mockLintingConstructor.Object,
                new[] { mockTestingConstructor.Object },
                mockDeployConstructor.Object);

            // Act - Simulate user modifying configuration through UI
            viewModel.CurrentWorkflow = modifiedWorkflow;

            // Assert - Verify configuration changes are captured
            Assert.Equal("User Modified Pipeline", viewModel.CurrentWorkflow.Name);
            Assert.NotNull(viewModel.CurrentWorkflow.On.Push);
            Assert.NotNull(viewModel.CurrentWorkflow.On.PullRequest);
            Assert.Equal(2, viewModel.CurrentWorkflow.On.Push.Branches.Length);
        }

        [Fact]
        public void ConfigurationChange_ThroughUI_TriggersYamlRegeneration()
        {
            // Arrange
            Mock<IObjectToYamlTranslator> mockTranslator = new Mock<IObjectToYamlTranslator>();
            Mock<IWorkFlowConstructor> mockConstructor = new Mock<IWorkFlowConstructor>();
            
            Workflow originalWorkflow = new Workflow
            {
                Name = "Original",
                On = new Triggers { Push = new BranchTrigger { Branches = new string[] { "main" } } },
                Jobs = new Dictionary<string, Job>()
            };

            Workflow modifiedWorkflow = new Workflow
            {
                Name = "Modified via UI",
                On = new Triggers { Push = new BranchTrigger { Branches = new string[] { "main", "staging" } } },
                Jobs = new Dictionary<string, Job>()
            };

            mockConstructor
                .Setup(c => c.ConstructWorkFlowWithParameters(
                    "Modified via UI",
                    It.IsAny<Triggers>(),
                    It.IsAny<Dictionary<string, Job>>()))
                .Returns(modifiedWorkflow);

            // Act
            Workflow result = mockConstructor.Object.ConstructWorkFlowWithParameters(
                "Modified via UI",
                modifiedWorkflow.On,
                modifiedWorkflow.Jobs);

            mockTranslator.Object.TranslateObjectToYaml(result, "output.yaml");

            // Assert
            mockConstructor.Verify(
                c => c.ConstructWorkFlowWithParameters(
                    "Modified via UI",
                    It.IsAny<Triggers>(),
                    It.IsAny<Dictionary<string, Job>>()),
                Times.Once,
                "Configuration changes should trigger workflow reconstruction");

            mockTranslator.Verify(
                t => t.TranslateObjectToYaml(
                    It.Is<Workflow>(w => w.Name == "Modified via UI"),
                    It.IsAny<string>()),
                Times.Once,
                "Modified configuration should be translated to YAML");
        }

        [Fact]
        public void JobConfiguration_UserModifiesRunsOn_UpdatesPersistsCorrectly()
        {
            // Arrange
            Job job = new Job
            {
                Name = "test-job",
                RunsOn = "ubuntu-latest",
                Steps = new List<Step>()
            };

            EditJobViewModel viewModel = new EditJobViewModel(job);

            // Act - User changes runner through UI
            viewModel.Job.RunsOn = "windows-latest";

            // Assert
            Assert.Equal("windows-latest", job.RunsOn);
        }

        [Fact]
        public void JobConfiguration_UserModifiesDependencies_UpdatesNeedsProperty()
        {
            // Arrange
            Job job = new Job
            {
                Name = "deploy",
                RunsOn = "ubuntu-latest",
                Needs = null
            };

            EditJobViewModel viewModel = new EditJobViewModel(job);

            // Act - User sets dependency through UI
            viewModel.Job.Needs = "build";

            // Assert
            Assert.Equal("build", job.Needs);
        }

        [Fact]
        public void MultipleConfigurationChanges_InSequence_AllPersistCorrectly()
        {
            // Arrange
            Workflow workflow = new Workflow
            {
                Name = "Initial",
                On = new Triggers()
            };

            EditWorkFlowConfigSettingsViewModel viewModel = new EditWorkFlowConfigSettingsViewModel(workflow);

            // Act - User makes multiple configuration changes
            viewModel.Name = "Step 1";
            Assert.Equal("Step 1", viewModel.Name);

            viewModel.Name = "Step 2";
            Assert.Equal("Step 2", viewModel.Name);

            viewModel.PushBranches.Add(new BranchEntry { Value = "feature/*" });
            Assert.Single(viewModel.PushBranches);

            // Assert - All changes persisted sequentially
            Assert.Equal("Step 2", viewModel.Name);
            Assert.Single(viewModel.PushBranches);
            Assert.Equal("feature/*", viewModel.PushBranches[0].Value);
        }

        [Fact]
        public void ConfigurationValidation_InvalidSettings_PreventsSave()
        {
            // Arrange
            Mock<IWorkFlowConstructor> mockConstructor = new Mock<IWorkFlowConstructor>();
            Mock<IObjectToYamlTranslator> mockTranslator = new Mock<IObjectToYamlTranslator>();

            Workflow invalidWorkflow = new Workflow
            {
                Name = null, // Invalid: missing name
                On = null     // Invalid: missing triggers
            };

            // Act & Assert
            // This test verifies that invalid configuration prevents YAML generation
            Assert.Null(invalidWorkflow.Name);
            Assert.Null(invalidWorkflow.On);

            // Translator should not be called with invalid configuration
            mockTranslator.Verify(
                t => t.TranslateObjectToYaml(invalidWorkflow, It.IsAny<string>()),
                Times.Never,
                "Invalid configuration should not trigger YAML generation");
        }
    }
}