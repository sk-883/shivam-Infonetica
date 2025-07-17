using System.Collections.Concurrent;
using System.Collections.Generic;
using ConfigurableWorkflowEngine.Domain.Models;

namespace ConfigurableWorkflowEngine.Repositories
{
    public class WorkflowDefinitionRepository : IWorkflowDefinitionRepository
    {
        private readonly ConcurrentDictionary<string, WorkflowDefinition> _store
            = new();

        public bool Add(WorkflowDefinition def) =>
            _store.TryAdd(def.Id, def);

        public WorkflowDefinition? Get(string id) =>
            _store.TryGetValue(id, out var def) ? def : null;

        public IEnumerable<WorkflowDefinition> List() =>
            _store.Values;
    }
}
