using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Application.Ports
{
    public interface ITestingJobConstructor
    {
        public Job ConstructTestingJobNoParameters();
        public Job ConstructTestingJobWithName(string Name);
        public Job ConstructTestingJobWithFullParameters(string name, string runsOn, Step steps);
    }
}
