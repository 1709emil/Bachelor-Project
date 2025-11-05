using YamlDotNet.Serialization;

namespace VisualWorkflowBuilder.Core.Entities;

public sealed class Triggers
{
    public BranchTrigger? Push {get; set;}
    
    [YamlMember(Alias = "pull_request", ApplyNamingConventions = false)]
    public BranchTrigger? PullRequest  {get; set;}

    
    
   
}