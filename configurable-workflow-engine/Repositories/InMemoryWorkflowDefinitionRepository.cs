using System.Collections.Concurrent;
using System.Collections.Generic;
using ConfigurableWorkflowEngine.Models;

namespace ConfigurableWorkflowEngine.Repositories
{
    public class InMemoryWorkflowDefinitionRepository : IWorkflowDefinitionRepository
    {
        private readonly ConcurrentDictionary<string, WorkflowDefinition> _defs = new();

        public void Add(WorkflowDefinition definition)
        {
            if (!_defs.TryAdd(definition.Id, definition))
                throw new InvalidOperationException($"Definition with ID '{definition.Id}' already exists.");
        }

        public WorkflowDefinition? Get(string id)
            => _defs.TryGetValue(id, out var def) ? def : null;

        public IEnumerable<WorkflowDefinition> GetAll()
            => _defs.Values;
    }
}
