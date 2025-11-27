using System.IO;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VisualWorkflowBuilder.Infrastructure.YamlTranslator;

public class ObjectToYamlImplementation : IObjectToYamlTranslator
{
    public void TranslateObjectToYaml(Workflow workflow)
    {
        string OutPutPath = "C:\\Programmering\\YamlTestingForBachelor\\.github\\workflows\\";
        
        var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithQuotingNecessaryStrings()
            .Build();
        string Yaml = serializer.Serialize(workflow);
        File.WriteAllText(Path.Combine(OutPutPath, "TestOfVisualBuilder.yaml"), Yaml);
        Console.WriteLine(Yaml);
    }
    
    
    
    
}