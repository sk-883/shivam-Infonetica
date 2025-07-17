using System.Collections.Generic;
using ConfigurableWorkflowEngine.Domain.Models;

namespace ConfigurableWorkflowEngine.Repositories
{
    public interface IWorkflowDefinitionRepository
    {
        bool Add(WorkflowDefinition def);
        WorkflowDefinition? Get(string id);
        IEnumerable<WorkflowDefinition> List();
    }
}
