using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports
{
    public interface IWorkFlowConstructor
    {
        public Workflow ConstructWorkFlowNoParameters();
        public Workflow ConstructWorkFlowWithParameters(string name, Triggers triggers, Dictionary<string, Job> jobs);
    }
}
