using System.IO;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports;

public interface IYamlToObjectTranslator
{
    public Workflow TranslateYamlToObject(FileStream stream);
}