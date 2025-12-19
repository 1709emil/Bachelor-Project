using System.IO;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VisualWorkflowBuilder.Infrastructure.YamlTranslator
{
    public class YamlToObjectImplementation : IYamlToObjectTranslator
    {
        public Workflow TranslateYamlToObject(FileStream stream)
        {
            using StreamReader reader = new StreamReader(stream);
            string yamlContent = reader.ReadToEnd();

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            Workflow workflow = deserializer.Deserialize<Workflow>(yamlContent);


            if (workflow?.Jobs != null)
            {
                foreach (var job in workflow.Jobs)
                {
                    if (job.Value?.Steps != null)
                    {
                        foreach (var step in job.Value.Steps)
                        {

                            if (step.Run != null && step.Run is string runString)
                            {
                                step.Run = runString.Trim();
                            }
                        }
                    }
                }
            }

            return workflow;
        }
    }
}
