using System.Collections.Generic;
using ConfigurableWorkflowEngine.Models;

namespace ConfigurableWorkflowEngine.Repositories
{
    public interface IWorkflowDefinitionRepository
    {
        void Add(WorkflowDefinition definition);
        WorkflowDefinition? Get(string id);
        IEnumerable<WorkflowDefinition> GetAll();
    }
}
