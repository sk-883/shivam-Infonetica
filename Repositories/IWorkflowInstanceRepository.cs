using ConfigurableWorkflowEngine.Models;
using System.Collections.Generic;

namespace ConfigurableWorkflowEngine.Repositories
{
    public interface IWorkflowInstanceRepository
    {
        IEnumerable<WorkflowInstance> GetAll();
        WorkflowInstance Get(string id);
        WorkflowInstance Create(string definitionId);
        void ExecuteAction(string instanceId, string actionId);
    }
}
