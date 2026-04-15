using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace GivingChampion.Common.Pagination;

[DebuggerDisplay("PageNumber = {PageNumber}, PageSize = {PageSize}")]
public sealed class PageParameters
{
    public const int MaxPageSize = 100;

    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    private int _pageSize = 20;

    [Range(1, MaxPageSize)]
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}