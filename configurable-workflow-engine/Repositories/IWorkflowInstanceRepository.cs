using System.Collections.Generic;
using ConfigurableWorkflowEngine.Models;

namespace ConfigurableWorkflowEngine.Repositories
{
    public interface IWorkflowInstanceRepository
    {
        void Add(WorkflowInstance instance);
        WorkflowInstance? Get(string id);
        void Update(WorkflowInstance instance);
        IEnumerable<WorkflowInstance> GetAll();
    }
}
