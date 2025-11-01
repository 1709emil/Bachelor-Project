using System.IO;
using VisualWorkflowBuilder.Core;

namespace VisualWorkflowBuilder.Abstractions;

public interface IYamlToObjectTranslator
{
    public Workflow TranslateYamlToObject(FileStream stream);
}