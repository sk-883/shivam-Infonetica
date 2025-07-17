using ConfigurableWorkflowEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigurableWorkflowEngine.Repositories
{
    public class WorkflowDefinitionRepository : IWorkflowDefinitionRepository
    {
        private readonly Dictionary<string, WorkflowDefinition> _definitions = new();

        public IEnumerable<WorkflowDefinition> GetAll() => _definitions.Values;

        public WorkflowDefinition Get(string id)
            => _definitions.TryGetValue(id, out var def) ? def : null!;

        public void Create(WorkflowDefinition definition)
        {
            if (_definitions.ContainsKey(definition.Id))
                throw new InvalidOperationException($"Definition '{definition.Id}' already exists.");

            Validate(definition);
            _definitions[definition.Id] = definition;
        }

        private void Validate(WorkflowDefinition def)
        {
            if (def.States.GroupBy(s => s.Id).Any(g => g.Count() > 1))
                throw new InvalidOperationException("State IDs must be unique.");

            if (def.Actions.GroupBy(a => a.Id).Any(g => g.Count() > 1))
                throw new InvalidOperationException("Action IDs must be unique.");

            if (def.States.Count(s => s.IsInitial) != 1)
                throw new InvalidOperationException("There must be exactly one initial state.");

            var ids = def.States.Select(s => s.Id).ToHashSet();
            foreach (var a in def.Actions)
            {
                if (!ids.Contains(a.ToState))
                    throw new InvalidOperationException($"Action '{a.Id}' → unknown ToState '{a.ToState}'.");
                if (a.FromStates.Any(fs => !ids.Contains(fs)))
                    throw new InvalidOperationException($"Action '{a.Id}' has unknown FromStates.");
            }
        }
    }
}
