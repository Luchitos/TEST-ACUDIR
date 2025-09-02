namespace Application.Common.Pagination
{
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int Page,
        int Size,
        int TotalItems,
        int TotalPages,
        string? SortBy,
        string? SortDir
    );
}
