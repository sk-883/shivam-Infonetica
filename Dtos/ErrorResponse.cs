namespace ConfigurableWorkflowEngine.Dtos
{
    public record ErrorResponse(
        string Error,
        string Code,
        string Details,
        DateTime Timestamp
    );
}
