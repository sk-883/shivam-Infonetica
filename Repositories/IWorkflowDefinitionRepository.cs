using ConfigurableWorkflowEngine.Models;
using System.Collections.Generic;

namespace ConfigurableWorkflowEngine.Repositories
{
    public interface IWorkflowDefinitionRepository
    {
        IEnumerable<WorkflowDefinition> GetAll();
        WorkflowDefinition Get(string id);
        void Create(WorkflowDefinition definition);
    }
}
