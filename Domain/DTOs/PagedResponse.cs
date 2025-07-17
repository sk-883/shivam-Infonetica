using System.Collections.Generic;

namespace ConfigurableWorkflowEngine.DTOs
{
    public record PagedResponse<T>(
        IEnumerable<T> Items,
        int Total,
        int PageNumber,
        int PageSize
    );
}
