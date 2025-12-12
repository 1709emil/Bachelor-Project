using System.Diagnostics;
using System.IO;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VisualWorkflowBuilder.Infrastructure.YamlTranslator;

public class ObjectToYamlImplementation : IObjectToYamlTranslator
{
    public void TranslateObjectToYaml(Workflow workflow, string outputPath)
    {
        string[] runStrings;
        foreach (var job in workflow.Jobs!)
        {
            foreach (var step in job.Value.Steps!)
            {
                
                if (step.Run != null && step.Run is string )
                {
                    runStrings = step.Run
                           .Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries)
                           .Select(l => l.Trim())
                           .ToArray();
                    step.Run = MutilineRunCmd(runStrings);
                   
                }
            }
        }

        var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithQuotingNecessaryStrings()
            .Build();
        string Yaml = serializer.Serialize(workflow);
        File.WriteAllText(outputPath, Yaml);
        Debug.WriteLine(Yaml);
    }


    private string MutilineRunCmd(string[] cmd)
    {
        string runCmd = string.Join("\n", cmd);
        return runCmd;
    }


}