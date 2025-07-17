using System;
using System.Collections.Generic;

namespace ConfigurableWorkflowEngine.Domain.Models
{
    public class WorkflowInstance
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public string DefinitionId { get; init; } = default!;
        public string CurrentStateId { get; set; } = default!;
        public List<HistoryRecord> History { get; } = new();
    }
}
