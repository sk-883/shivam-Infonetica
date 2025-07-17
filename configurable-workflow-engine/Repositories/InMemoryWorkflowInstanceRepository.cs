using System.Collections.Concurrent;
using System.Collections.Generic;
using ConfigurableWorkflowEngine.Models;

namespace ConfigurableWorkflowEngine.Repositories
{
    public class InMemoryWorkflowInstanceRepository : IWorkflowInstanceRepository
    {
        private readonly ConcurrentDictionary<string, WorkflowInstance> _instances = new();

        public void Add(WorkflowInstance instance)
        {
            if (!_instances.TryAdd(instance.Id, instance))
                throw new InvalidOperationException($"Instance with ID '{instance.Id}' already exists.");
        }

        public WorkflowInstance? Get(string id)
            => _instances.TryGetValue(id, out var inst) ? inst : null;

        public void Update(WorkflowInstance instance)
        {
            _instances[instance.Id] = instance;
        }

        public IEnumerable<WorkflowInstance> GetAll()
            => _instances.Values;
    }
}
