using AutoMapper;
using GivingChampion.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Common.Extensions.Mapper
{
    public static class MapperExtensions
    {
        /// <summary>
        /// Maps a PagedList<TSource> to PagedList<TDestination> while preserving all pagination metadata.
        /// This is the recommended way to handle paged results with AutoMapper (avoids generic converter instantiation issues).
        /// </summary>
        public static PagedList<TDestination> MapPagedList<TSource, TDestination>(
            this IMapper mapper,
            PagedList<TSource> source)
        {
            if (source == null)
                return null;

            var mappedItems = mapper.Map<List<TDestination>>(source.Items);

            return new PagedList<TDestination>(
                mappedItems,
                source.PageNumber,
                source.PageSize,
                source.TotalCount
            );
        }
    }
}
