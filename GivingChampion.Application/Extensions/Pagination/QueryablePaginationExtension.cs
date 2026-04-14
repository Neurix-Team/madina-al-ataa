using GivingChampion.Application.Common.Pagination;

namespace GivingChampion.Application.Extensions.Pagination;

public static class QueryablePaginationExtension
{
    public static Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        PageParameters parameters,
        CancellationToken cancellationToken = default)
    {
        return PagedList<T>.CreateAsync(
            query,
            parameters.PageNumber,
            parameters.PageSize,
            cancellationToken);
    }
}