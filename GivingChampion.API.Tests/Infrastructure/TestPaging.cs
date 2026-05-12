using GivingChampion.Common.Pagination;

namespace GivingChampion.API.Tests.Infrastructure;

internal static class TestPaging
{
    public static PagedList<T> CreatePage<T>(params T[] items)
        => new(items, 1, items.Length == 0 ? 10 : items.Length, items.Length);
}
