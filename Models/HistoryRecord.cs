namespace ConfigurableWorkflowEngine.Models
{
    public class HistoryRecord
    {
        public string ActionId { get; set; }
        public string FromState { get; set; }
        public string ToState { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
