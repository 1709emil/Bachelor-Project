using System.IO;
using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.Abstractions;

public interface IObjectToYamlTranslator
{
    public void TranslateObjectToYaml(Workflow workflow);
}