using System.IO;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.Infrastructure.JobConstruction;
using VisualWorkflowBuilder.Infrastructure.WorkflowConstructor;
using VisualWorkflowBuilder.Infrastructure.YamlTranslator;
using Xunit;

namespace Test.IntegrationTests
{
    public class SystemAvailabilityIntegrationTests : IDisposable
    {
        private readonly List<string> _tempFiles = new List<string>();

        [Fact]
        public void EndToEndPipeline_ShouldMaintain99PercentAvailability_UnderNormalLoad()
        {
            // Arrange
            int totalIterations = 1000;
            int successCount = 0;
            List<Exception> failures = new List<Exception>();

            // Act
            for (int i = 0; i < totalIterations; i++)
            {
                try
                {
                    // Complete end-to-end workflow
                    var workflow = CreateCompleteWorkflow($"Test Workflow {i}");
                    var yamlPath = Path.GetTempFileName();
                    _tempFiles.Add(yamlPath);

                    // Translate to YAML
                    IObjectToYamlTranslator toYaml = new ObjectToYamlImplementation();
                    toYaml.TranslateObjectToYaml(workflow, yamlPath);

                    // Read back from YAML
                    IYamlToObjectTranslator fromYaml = new YamlToObjectImplementation();
                    using (FileStream stream = new FileStream(yamlPath, FileMode.Open, FileAccess.Read))
                    {
                        var restored = fromYaml.TranslateYamlToObject(stream);
                        
                        // Validate roundtrip
                        if (restored != null && restored.Jobs != null && restored.Jobs.Count > 0)
                        {
                            successCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    failures.Add(ex);
                }
            }

            // Assert - 99% success rate
            double successRate = (double)successCount / totalIterations;
            Assert.True(successRate >= 0.99, 
                $"System availability {successRate:P2} is below 99% requirement. " +
                $"Failures: {failures.Count}, Sample error: {failures.FirstOrDefault()?.Message}");
        }

        [Fact]
        public async Task EndToEndPipeline_ShouldHandleConcurrentOperations_With99PercentSuccess()
        {
            // Arrange
            int concurrentOperations = 200;
            int successCount = 0;
            List<Exception> failures = new List<Exception>();
            var lockObject = new object();

            // Act
            var tasks = Enumerable.Range(0, concurrentOperations).Select(async i =>
            {
                await Task.Yield();
                try
                {
                    var workflow = CreateCompleteWorkflow($"Concurrent Workflow {i}");
                    var yamlPath = Path.GetTempFileName();
                    
                    lock (lockObject)
                    {
                        _tempFiles.Add(yamlPath);
                    }

                    IObjectToYamlTranslator toYaml = new ObjectToYamlImplementation();
                    toYaml.TranslateObjectToYaml(workflow, yamlPath);

                    IYamlToObjectTranslator fromYaml = new YamlToObjectImplementation();
                    using (FileStream stream = new FileStream(yamlPath, FileMode.Open, FileAccess.Read))
                    {
                        var restored = fromYaml.TranslateYamlToObject(stream);
                        if (restored != null)
                        {
                            lock (lockObject)
                            {
                                successCount++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (lockObject)
                    {
                        failures.Add(ex);
                    }
                }
            });

            await Task.WhenAll(tasks);

            // Assert
            double successRate = (double)successCount / concurrentOperations;
            Assert.True(successRate >= 0.99, 
                $"Concurrent availability {successRate:P2} is below 99%. Failures: {failures.Count}");
        }

        [Fact]
        public void MultiPlatformWorkflows_ShouldMaintainAvailability_AcrossAllJobTypes()
        {
            // Arrange
            int iterations = 500;
            int successCount = 0;
            var jobConstructorTypes = new List<string> { "DotNet", "Maven", "JUnit", "Deploy", "Lint" };

            // Act
            foreach (var type in jobConstructorTypes)
            {
                for (int i = 0; i < iterations / jobConstructorTypes.Count; i++)
                {
                    try
                    {
                        var workflow = CreateWorkflowForJobType(type, i);
                        var yamlPath = Path.GetTempFileName();
                        _tempFiles.Add(yamlPath);

                        IObjectToYamlTranslator translator = new ObjectToYamlImplementation();
                        translator.TranslateObjectToYaml(workflow, yamlPath);

                        if (File.Exists(yamlPath) && new FileInfo(yamlPath).Length > 0)
                        {
                            successCount++;
                        }
                    }
                    catch
                    {
                        // Count failure
                    }
                }
            }

            // Assert
            double successRate = (double)successCount / iterations;
            Assert.True(successRate >= 0.99, 
                $"Multi-platform availability {successRate:P2} is below 99%");
        }

        [Fact]
        public void ComplexWorkflowPipeline_WithDependencies_MaintainsAvailability()
        {
            // Arrange
            int iterations = 500;
            int successCount = 0;

            // Act
            for (int i = 0; i < iterations; i++)
            {
                try
                {
                    // Build complex workflow with dependencies
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

                    Workflow workflow = workflowConstructor.ConstructWorkFlowWithParameters(
                        $"Full Pipeline {i}",
                        new Triggers { Push = new BranchTrigger { Branches = new string[] { "main" } } },
                        jobs);

                    // Translate and validate
                    var yamlPath = Path.GetTempFileName();
                    _tempFiles.Add(yamlPath);

                    IObjectToYamlTranslator toYaml = new ObjectToYamlImplementation();
                    toYaml.TranslateObjectToYaml(workflow, yamlPath);

                    IYamlToObjectTranslator fromYaml = new YamlToObjectImplementation();
                    using (FileStream stream = new FileStream(yamlPath, FileMode.Open, FileAccess.Read))
                    {
                        var restored = fromYaml.TranslateYamlToObject(stream);
                        
                        // Validate dependencies preserved
                        if (restored?.Jobs != null && 
                            restored.Jobs.Count == 4 &&
                            restored.Jobs["Build"].Needs == "Lint" &&
                            restored.Jobs["Test"].Needs == "Build" &&
                            restored.Jobs["Deploy"].Needs == "Test")
                        {
                            successCount++;
                        }
                    }
                }
                catch
                {
                    // Count failure
                }
            }

            // Assert
            double successRate = (double)successCount / iterations;
            Assert.True(successRate >= 0.99, 
                $"Complex pipeline availability {successRate:P2} is below 99%");
        }

        [Fact]
        public void SystemResilience_HandlesInvalidInput_WithoutSystemFailure()
        {
            // Arrange
            int totalOperations = 500;
            int systemStillResponsive = 0;
            var invalidInputs = GenerateInvalidInputs();

            // Act
            foreach (var invalidInput in invalidInputs.Take(totalOperations))
            {
                try
                {
                    // Try to process invalid input
                    var yamlPath = Path.GetTempFileName();
                    _tempFiles.Add(yamlPath);
                    File.WriteAllText(yamlPath, invalidInput);

                    IYamlToObjectTranslator translator = new YamlToObjectImplementation();
                    using (FileStream stream = new FileStream(yamlPath, FileMode.Open, FileAccess.Read))
                    {
                        translator.TranslateYamlToObject(stream);
                    }
                }
                catch
                {
                    // Expected to fail, but system should continue
                }

                // Verify system still works after invalid input
                try
                {
                    var validWorkflow = CreateCompleteWorkflow("Recovery Test");
                    var testPath = Path.GetTempFileName();
                    _tempFiles.Add(testPath);

                    IObjectToYamlTranslator toYaml = new ObjectToYamlImplementation();
                    toYaml.TranslateObjectToYaml(validWorkflow, testPath);

                    if (File.Exists(testPath))
                    {
                        systemStillResponsive++;
                    }
                }
                catch
                {
                    // System failed to recover
                }
            }

            // Assert - System should recover 99% of the time
            double recoveryRate = (double)systemStillResponsive / totalOperations;
            Assert.True(recoveryRate >= 0.99, 
                $"System recovery rate {recoveryRate:P2} is below 99%");
        }

        private Workflow CreateCompleteWorkflow(string name)
        {
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

            return workflowConstructor.ConstructWorkFlowWithParameters(
                name,
                new Triggers { Push = new BranchTrigger { Branches = new string[] { "main" } } },
                jobs);
        }

        private Workflow CreateWorkflowForJobType(string jobType, int iteration)
        {
            IWorkFlowConstructor workflowConstructor = new WorkFlowConstructorImplementation();
            Job job = jobType switch
            {
                "DotNet" => new DotNetTestingJobImplementation().ConstructTestingJobNoParameters(),
                "Maven" => new MavenBuildJobImplementation().ConstructBuildJobNoParameters(),
                "JUnit" => new JUnitTestingJobImplementation().ConstructTestingJobNoParameters(),
                "Deploy" => new DeployJobImplementation().ConstructDeployJobNoParameters(),
                "Lint" => new LintingJobImplementation().ConstructLintingJobNoParameters(),
                _ => new DotNetBuildJobImplementation().ConstructBuildJobNoParameters()
            };

            return workflowConstructor.ConstructWorkFlowWithParameters(
                $"{jobType} Workflow {iteration}",
                new Triggers(),
                new Dictionary<string, Job> { { job.Name, job } });
        }

        private IEnumerable<string> GenerateInvalidInputs()
        {
            yield return "invalid: yaml: content: [[[";
            yield return "";
            yield return "name: Test\njobs:\n  build:\n    invalid_field: true";
            yield return "completely invalid content 12345 !@#$%";
            yield return "name:\njobs:";
            
            for (int i = 0; i < 1000; i++)
            {
                yield return $"invalid_{i}: test";
            }
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