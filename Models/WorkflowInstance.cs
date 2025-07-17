namespace ConfigurableWorkflowEngine.Models
{
    public class WorkflowInstance
    {
        public string Id { get; set; }
        public string DefinitionId { get; set; }
        public string CurrentState { get; set; }
        public List<HistoryRecord> History { get; set; } = new();

        public WorkflowInstance(string id, string definitionId, string initialState)
        {
            Id = id;
            DefinitionId = definitionId;
            CurrentState = initialState;
        }
    }
}
