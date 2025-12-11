using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports
{
    public interface IDelpoyJobConstructor
    {
        public Job ConstructDeployJobNoParameters();
        public Job ConstructDeployJobWithName(string Name);
        public Job ConstructDeployJobWithFullParameters(string name, string runsOn, Step steps);
    }
}
