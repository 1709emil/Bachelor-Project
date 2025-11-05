using YamlDotNet.Serialization;

namespace VisualWorkflowBuilder.Core;

public sealed class Job
{
    [YamlMember(Order = 1)]
    public string Name  {get; set;}
    
    [YamlMember(Alias = "runs-on",  ApplyNamingConventions = false,Order = 2)]
    public string? RunsOn  {get; set;}
    
    [YamlMember(Order = 3)]
    public string? Needs  {get; set;}
    
    [YamlMember(Order = 4)]
    public Dictionary<string, string>? Env  {get; set;}
    
    [YamlMember(Order = 5)]
    public List<Step>? Steps  {get; set;} 
    
  
}