// File: Dtos/ErrorResponse.cs
namespace WorkflowEngine.Dtos
{
    public record ErrorResponse(
        string Error,
        string Code,
        string Details,
        DateTime Timestamp
    );
}
