using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports
{
    public interface ILintingJobConstructor
    {
        public Job ConstructLintingJobNoParameters();
        public Job ConstructLintingJobWithName(string Name);
        public Job ConstructLintingJobWithFullParameters(string name, string runsOn, Step steps);
    }
}
