using VisualWorkflowBuilder.Abstractions;
using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.YamlTranslator;

public class ObjectToYamlService
{
    private IObjectToYamlTranslator _translator;
    
    public ObjectToYamlService(IObjectToYamlTranslator translator)
    {
        _translator = translator;
    }

    public void test()
    {
        _translator.TranslateObjectToYaml(TestingWorkflow2());
    }
    
    private Workflow TestingWorkflow()
    {//method starting bracket
     
        var wf = new Workflow
        {
            Name = "CI",
            On = new Triggers
            {
                Push = new BranchTrigger { Branches = new[] { "main" } },
                PullRequest = new BranchTrigger { Branches = new[] { "main" } }
            },
            Jobs = {
                ["build"] = new Job {
                    Name = "Build & Test",
                    RunsOn = "ubuntu-latest",
                    Env = new() { ["NODE_ENV"] = "test" },
                    Steps = {
                        new Step { Name = "Checkout", Uses = "actions/checkout@v4" },
                        new Step {
                            Name = "Setup Node",
                            Uses = "actions/setup-node@v4",
                            With = new() { ["node-version"] = "20" }
                        },
                        new Step { Name = "Install", Run = "npm ci" },
                        new Step {
                            Name = "Test",
                            Run = MutilineRunCmd(new[]
                            { 
                                "npm ci",
                                "npm test -- --reporter=junit",
                                "echo \"ref=${{ github.ref }}\""
                            }
                            )
                        }
                    }
                }
            }
        };

        return wf;
    }//method ending bracket


    private Workflow TestingWorkflow2()
    {
        var wf = new Workflow
        {
            Name = "CI",
            On = new Triggers
            {
                Push = new BranchTrigger { Branches = new[] { "main" } },
                PullRequest = new BranchTrigger { Branches = new[] { "main" } }
            },
            Jobs = {
                ["build"] = new Job {
                    Name = "Say Hello",
                    RunsOn = "ubuntu-latest",
                    Steps = {
                        new Step {
                            Name = "Checkout (optional for empty repo)",
                            Uses = "actions/checkout@v4"
                        },
                        new Step {
                            Name = "Say hi",
                            Run = "echo \"Hello from GitHub Actions!\""
                        }
                    }
                }
            }
        };
        return wf;
    }
    private string MutilineRunCmd(string[] cmd)
    {
        string runCmd = string.Join("\n", cmd);
        return runCmd;
    }
}