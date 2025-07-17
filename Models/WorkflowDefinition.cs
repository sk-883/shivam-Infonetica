namespace ConfigurableWorkflowEngine.Models
{
    public class WorkflowDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<State> States { get; set; } = new();
        public List<WorkflowAction> Actions { get; set; } = new();
    }
}
