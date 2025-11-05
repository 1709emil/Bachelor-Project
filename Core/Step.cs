using YamlDotNet.Serialization;

namespace VisualWorkflowBuilder.Core;

public sealed class Step
{
    [YamlMember(Order = 1)]
    public string? Name  {get; set;}
    
    [YamlMember(Order = 2)]
    public string? Uses  {get; set;}
    
    [YamlMember(Order = 3)]
    public string? Run  {get; set;}
    
    [YamlMember(Order = 4)]
    public Dictionary<string, string>? With  {get; set;}
    
    [YamlMember(Order = 5)]
    public Dictionary<string, string>? Env  {get; set;}
    


}