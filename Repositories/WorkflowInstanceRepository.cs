using System.Collections.Concurrent;
using System.Collections.Generic;
using ConfigurableWorkflowEngine.Domain.Models;

namespace ConfigurableWorkflowEngine.Repositories
{
    public class WorkflowInstanceRepository : IWorkflowInstanceRepository
    {
        private readonly ConcurrentDictionary<string, WorkflowInstance> _store
            = new();

        public bool Add(WorkflowInstance inst) =>
            _store.TryAdd(inst.Id, inst);

        public WorkflowInstance? Get(string id) =>
            _store.TryGetValue(id, out var inst) ? inst : null;

        public IEnumerable<WorkflowInstance> List() =>
            _store.Values;
    }
}
