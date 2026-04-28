using GivingChampion.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace GivingChampion.Common.Extensions.SoftDelete
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Applies global query filter to automatically exclude soft-deleted entities
        /// </summary>
        public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    var property = Expression.Property(
                        parameter,
                        nameof(ISoftDeletable.IsDeleted)
                    );

                    var filter = Expression.Lambda(
                        Expression.Equal(
                            property,
                            Expression.Constant(false, typeof(bool))
                        ),
                        parameter
                    );

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }
    }
}