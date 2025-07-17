using System;
using System.Collections.Generic;

namespace ConfigurableWorkflowEngine.Domain.Models
{
    public class WorkflowDefinition
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public List<State> States { get; init; } = new();
        public List<ActionDefinition> Actions { get; init; } = new();
    }
}
