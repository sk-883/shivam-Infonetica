namespace ConfigurableWorkflowEngine.Domain.Models
{
    public record ActionDefinition(
        string Id,
        string Name,
        bool Enabled,
        List<string> FromStateIds,
        string ToStateId
    );
}
