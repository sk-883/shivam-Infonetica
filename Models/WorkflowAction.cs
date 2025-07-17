namespace ConfigurableWorkflowEngine.Models
{
    public class WorkflowAction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; } = true;
        public List<string> FromStates { get; set; }
        public string ToState { get; set; }
    }
}
