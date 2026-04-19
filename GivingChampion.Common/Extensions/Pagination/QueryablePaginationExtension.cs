using GivingChampion.Common.Pagination;

namespace GivingChampion.Common.Extensions.Pagination
{
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
}

