namespace ConfigurableWorkflowEngine.Models
{
    public class WorkflowState
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsInitial { get; set; }
        public bool IsFinal { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
