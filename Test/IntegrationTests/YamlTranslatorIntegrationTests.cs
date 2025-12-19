using System.IO;
using System.Text;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.Infrastructure.YamlTranslator;
using Xunit;

namespace Test.IntegrationTests
{
    public class YamlTranslatorIntegrationTests : IDisposable
    {
        private readonly List<string> _tempFiles = new List<string>();

        [Fact]
        public void ObjectToYaml_TranslateObjectToYaml_CreatesValidYamlFile()
        {
            // Arrange
            IObjectToYamlTranslator translator = new ObjectToYamlImplementation();
            string outputPath = Path.GetTempFileName();
            _tempFiles.Add(outputPath);

            Workflow workflow = new Workflow
            {
                Name = "Test Workflow",
                On = new Triggers
                {
                    Push = new BranchTrigger { Branches = new string[] { "main" } }
                },
                Jobs = new Dictionary<string, Job>
                {
                    { "build", new Job { Name = "build", RunsOn = "ubuntu-latest", Steps = new List<Step>() } }
                }
            };

            // Act
            translator.TranslateObjectToYaml(workflow, outputPath);

            // Assert
            Assert.True(File.Exists(outputPath));
            string content = File.ReadAllText(outputPath);
            Assert.Contains("name: Test Workflow", content);
            Assert.Contains("build:", content);
        }

        [Fact]
        public void YamlToObject_TranslateYamlToObject_DeserializesCorrectly()
        {
            // Arrange
            IYamlToObjectTranslator translator = new YamlToObjectImplementation();
            string yamlContent = @"name: CI Pipeline
on:
  push:
    branches:
      - main
jobs:
  build:
    name: build
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v2";

            string tempFile = Path.GetTempFileName();
            _tempFiles.Add(tempFile);
            File.WriteAllText(tempFile, yamlContent);

            // Act
            Workflow workflow;
            using (FileStream stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
            {
                workflow = translator.TranslateYamlToObject(stream);
            }

            // Assert
            Assert.NotNull(workflow);
            Assert.Equal("CI Pipeline", workflow.Name);
            Assert.NotNull(workflow.On);
            Assert.NotNull(workflow.Jobs);
            Assert.True(workflow.Jobs.ContainsKey("build"));
        }

        [Fact]
        public void YamlTranslators_RoundTrip_PreservesData()
        {
            // Arrange
            IObjectToYamlTranslator toYaml = new ObjectToYamlImplementation();
            IYamlToObjectTranslator fromYaml = new YamlToObjectImplementation();

            string tempFile = Path.GetTempFileName();
            _tempFiles.Add(tempFile);

            Workflow original = new Workflow
            {
                Name = "Integration Test",
                On = new Triggers
                {
                    Push = new BranchTrigger { Branches = new string[] { "main", "develop" } }
                },
                Jobs = new Dictionary<string, Job>
                {
                    {
                        "test",
                        new Job
                        {
                            Name = "test",
                            RunsOn = "ubuntu-latest",
                            Steps = new List<Step>
                            {
                                new Step { Name = "Checkout", Uses = "actions/checkout@v2" },
                                new Step { Name = "Test", Run = "npm test" }
                            }
                        }
                    }
                }
            };

            // Act
            toYaml.TranslateObjectToYaml(original, tempFile);

            Workflow restored;
            using (FileStream stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
            {
                restored = fromYaml.TranslateYamlToObject(stream);
            }

            // Assert
            Assert.Equal(original.Name, restored.Name);
            Assert.NotNull(restored.Jobs);
            Assert.True(restored.Jobs.ContainsKey("test"));
            Assert.Equal(2, restored.Jobs["test"].Steps!.Count);
        }

        [Fact]
        public void ObjectToYaml_HandlesMultilineRunCommands_FormatsCorrectly()
        {
            // Arrange
            IObjectToYamlTranslator translator = new ObjectToYamlImplementation();
            string outputPath = Path.GetTempFileName();
            _tempFiles.Add(outputPath);

            Workflow workflow = new Workflow
            {
                Name = "Multiline Test",
                Jobs = new Dictionary<string, Job>
                {
                    {
                        "deploy",
                        new Job
                        {
                            Name = "deploy",
                            RunsOn = "ubuntu-latest",
                            Steps = new List<Step>
                            {
                                new Step
                                {
                                    Name = "Deploy",
                                    Run = "echo Deploying\nbash deploy.sh"
                                }
                            }
                        }
                    }
                }
            };

            // Act
            translator.TranslateObjectToYaml(workflow, outputPath);

            // Assert
            string content = File.ReadAllText(outputPath);
            Assert.Contains("echo Deploying", content);
            Assert.Contains("bash deploy.sh", content);
        }

        [Fact]
        public void YamlToObject_WithNeeds_ParsesDependencies()
        {
            // Arrange
            IYamlToObjectTranslator translator = new YamlToObjectImplementation();
            string yamlContent = @"name: Pipeline
jobs:
  build:
    name: build
    runs-on: ubuntu-latest
  test:
    name: test
    runs-on: ubuntu-latest
    needs: build";

            string tempFile = Path.GetTempFileName();
            _tempFiles.Add(tempFile);
            File.WriteAllText(tempFile, yamlContent);

            // Act
            Workflow workflow;
            using (FileStream stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
            {
                workflow = translator.TranslateYamlToObject(stream);
            }

            // Assert
            Assert.NotNull(workflow.Jobs);
            Assert.True(workflow.Jobs.ContainsKey("test"));
            Assert.Equal("build", workflow.Jobs["test"].Needs);
        }

        public void Dispose()
        {
            foreach (string file in _tempFiles)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
        }
    }
}