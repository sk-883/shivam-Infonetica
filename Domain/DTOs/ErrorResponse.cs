namespace ConfigurableWorkflowEngine.DTOs
{
    public record ErrorResponse(
        string Message,
        string Code,
        object? Details = null
    );
}
