using System;

namespace ConfigurableWorkflowEngine.Models
{
    public class TransitionRecord
    {
        public string ActionId { get; set; } = default!;
        public string FromState { get; set; } = default!;
        public string ToState { get; set; } = default!;
        public DateTime Timestamp { get; set; }
    }
}
