using System.Collections.Generic;

namespace ConfigurableWorkflowEngine.Models
{
    public class WorkflowInstance
    {
        public string Id { get; set; } = default!;
        public string DefinitionId { get; set; } = default!;
        public string CurrentState { get; set; } = default!;
        public List<TransitionRecord> History { get; set; } = new();
    }
}
