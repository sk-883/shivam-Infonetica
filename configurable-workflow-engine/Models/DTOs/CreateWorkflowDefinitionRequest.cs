using System.Collections.Generic;

namespace ConfigurableWorkflowEngine.Models.DTOs
{
    public class CreateWorkflowDefinitionRequest
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<WorkflowState> States { get; set; } = new();
        public List<WorkflowAction> Actions { get; set; } = new();
    }
}
