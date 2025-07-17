using WorkflowEngine.Models;

namespace WorkflowEngine.Repositories
{
    public interface IWorkflowDefinitionRepository
    {
        IEnumerable<WorkflowDefinition> GetAll();
        WorkflowDefinition Get(string id);
        void Create(WorkflowDefinition definition);
    }
}
