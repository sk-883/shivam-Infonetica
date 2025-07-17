using System.Collections.Generic;

namespace ConfigurableWorkflowEngine.Models
{
    public class WorkflowAction
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool Enabled { get; set; } = true;
        public List<string> FromStates { get; set; } = new();
        public string ToState { get; set; } = default!;
    }
}
