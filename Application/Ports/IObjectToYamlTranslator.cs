using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports;

public interface IObjectToYamlTranslator
{
    public void TranslateObjectToYaml(Workflow workflow);
}