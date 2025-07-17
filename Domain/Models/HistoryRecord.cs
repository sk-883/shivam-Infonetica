using System;

namespace ConfigurableWorkflowEngine.Domain.Models
{
    public record HistoryRecord(
        string ActionId,
        DateTime Timestamp = default
    )
    {
        public HistoryRecord() : this(string.Empty, DateTime.UtcNow) { }
        public HistoryRecord(string actionId) : this(actionId, DateTime.UtcNow) { }
    }
}
