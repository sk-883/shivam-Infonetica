using WorkflowEngine.Models;

namespace WorkflowEngine.Repositories
{
    public class WorkflowInstanceRepository : IWorkflowInstanceRepository
    {
        private readonly Dictionary<string, WorkflowInstance> _instances = new();
        private readonly IWorkflowDefinitionRepository _defRepo;

        public WorkflowInstanceRepository(IWorkflowDefinitionRepository defRepo)
            => _defRepo = defRepo;

        public IEnumerable<WorkflowInstance> GetAll() => _instances.Values;

        public WorkflowInstance? Get(string id)
            => _instances.TryGetValue(id, out var inst) ? inst : null;

        public WorkflowInstance Create(string definitionId)
        {
            var def = _defRepo.Get(definitionId)
                      ?? throw new InvalidOperationException($"Definition '{definitionId}' not found.");

            var init = def.States.Single(s => s.IsInitial).Id;
            var id   = Guid.NewGuid().ToString();
            var inst = new WorkflowInstance(id, definitionId, init);

            _instances[id] = inst;
            return inst;
        }

        public void ExecuteAction(string instanceId, string actionId)
        {
            var inst = Get(instanceId)
                       ?? throw new InvalidOperationException($"Instance '{instanceId}' not found.");
            var def = _defRepo.Get(inst.DefinitionId)!;
            var act = def.Actions.SingleOrDefault(a => a.Id == actionId)
                      ?? throw new InvalidOperationException($"Action '{actionId}' not in definition.");

            if (!act.Enabled)
                throw new InvalidOperationException($"Action '{actionId}' is disabled.");
            if (def.States.Single(s => s.Id == inst.CurrentState).IsFinal)
                throw new InvalidOperationException($"Instance already in final state '{inst.CurrentState}'.");
            if (!act.FromStates.Contains(inst.CurrentState))
                throw new InvalidOperationException($"Cannot fire '{actionId}' from '{inst.CurrentState}'.");

            inst.History.Add(new HistoryRecord {
                ActionId  = actionId,
                FromState = inst.CurrentState,
                ToState   = act.ToState,
                Timestamp = DateTime.UtcNow
            });
            inst.CurrentState = act.ToState;
        }
    }
}
