using WorkflowEngine.Models;

namespace WorkflowEngine.Repositories
{
    public interface IWorkflowInstanceRepository
    {
        IEnumerable<WorkflowInstance> GetAll();
        WorkflowInstance? Get(string id);
        WorkflowInstance Create(string definitionId);
        void ExecuteAction(string instanceId, string actionId);
    }
}
