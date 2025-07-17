using System.Collections.Generic;
using ConfigurableWorkflowEngine.Domain.Models;

namespace ConfigurableWorkflowEngine.Repositories
{
    public interface IWorkflowInstanceRepository
    {
        bool Add(WorkflowInstance inst);
        WorkflowInstance? Get(string id);
        IEnumerable<WorkflowInstance> List();
    }
}
