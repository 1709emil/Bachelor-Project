using YamlDotNet.Serialization;
namespace VisualWorkflowBuilder.Core;

public sealed class Workflow
{
    [YamlMember(Order = 1)]
    public string Name  {get; set;}
    
    [YamlMember(Alias = "on", ApplyNamingConventions = false, Order = 2)]
    public Triggers On  {get; set;}
    
    [YamlMember(Order = 3)]
    public Dictionary<string, Job> Jobs {get; set;}  = new();
    
    
    

}