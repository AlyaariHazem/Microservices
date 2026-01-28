using BuildingBlocks.Querying.Models;
using BuildingBlocks.Querying.Security;
using Marten.Linq;
using Marten.Pagination;
using System.Linq.Expressions;

namespace BuildingBlocks.Querying.Extensions
{
    /// <summary>
    /// Marten-specific query extensions for pagination
    /// </summary>
    public static class MartenQueryableExtensions
    {
        /// <summary>
        /// Applies query parameters (filtering, sorting, searching) and returns a Marten paged list
        /// </summary>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(
            this IMartenQueryable<T> query,
            QueryParameters parameters,
            ColumnMap<T> columns,
            CancellationToken cancellationToken = default)
        {
            // Apply filters, search, and sorting - Marten-specific implementation
            query = ApplyMartenFilters(query, parameters, columns);
            query = ApplyMartenSearch(query, parameters, columns);
            query = ApplyMartenSort(query, parameters, columns);
            
            // Use Marten's pagination
            return await query.ToPagedListAsync(
                parameters.Page,
                parameters.PageSize,
                cancellationToken);
        }

        private static IMartenQueryable<T> ApplyMartenFilters<T>(
            IMartenQueryable<T> query,
            QueryParameters parameters,
            ColumnMap<T> columns)
        {
            foreach (var filter in parameters.Filters)
            {
                if (!columns.TryGet(filter.Key, out var selector))
                    continue;

                query = ApplyMartenFilter(query, selector, filter.Value);
            }

            return query;
        }

        private static IMartenQueryable<T> ApplyMartenFilter<T>(
            IMartenQueryable<T> query,
            Expression<Func<T, object>> selector,
            string value)
        {
            // Extract the actual property expression from the selector
            Expression propertyAccess = selector.Body;
            if (propertyAccess is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                propertyAccess = unary.Operand;
            }

            // Build the filter expression
            var parameter = selector.Parameters[0];
            Expression filterExpression;

            // Handle string properties (including computed ones like string.Join)
            if (propertyAccess.Type == typeof(string))
            {
                var constant = Expression.Constant(value, typeof(string));
                var containsMethod = typeof(string).GetMethod(
                    nameof(string.Contains),
                    new[] { typeof(string) },
                    null);
                
                if (containsMethod != null)
                {
                    var containsCall = Expression.Call(propertyAccess, containsMethod, constant);
                    filterExpression = containsCall;
                }
                else
                {
                    // Fallback to equality
                    filterExpression = Expression.Equal(propertyAccess, constant);
                }
            }
            // Handle numeric types
            else if (propertyAccess.Type == typeof(int) || propertyAccess.Type == typeof(int?))
            {
                if (int.TryParse(value, out var intValue))
                {
                    var constant = Expression.Constant(intValue, propertyAccess.Type);
                    filterExpression = Expression.Equal(propertyAccess, constant);
                }
                else
                {
                    return query; // Skip invalid filter
                }
            }
            else if (propertyAccess.Type == typeof(decimal) || propertyAccess.Type == typeof(decimal?))
            {
                if (decimal.TryParse(value, out var decimalValue))
                {
                    var constant = Expression.Constant(decimalValue, propertyAccess.Type);
                    filterExpression = Expression.Equal(propertyAccess, constant);
                }
                else
                {
                    return query; // Skip invalid filter
                }
            }
            else if (propertyAccess.Type == typeof(double) || propertyAccess.Type == typeof(double?))
            {
                if (double.TryParse(value, out var doubleValue))
                {
                    var constant = Expression.Constant(doubleValue, propertyAccess.Type);
                    filterExpression = Expression.Equal(propertyAccess, constant);
                }
                else
                {
                    return query; // Skip invalid filter
                }
            }
            else if (propertyAccess.Type == typeof(bool) || propertyAccess.Type == typeof(bool?))
            {
                if (bool.TryParse(value, out var boolValue))
                {
                    var constant = Expression.Constant(boolValue, propertyAccess.Type);
                    filterExpression = Expression.Equal(propertyAccess, constant);
                }
                else
                {
                    return query; // Skip invalid filter
                }
            }
            else
            {
                // For other types, try string contains on ToString()
                var toStringMethod = propertyAccess.Type.GetMethod("ToString", Type.EmptyTypes);
                if (toStringMethod != null)
                {
                    var toStringCall = Expression.Call(propertyAccess, toStringMethod);
                    var constant = Expression.Constant(value, typeof(string));
                    var containsMethod = typeof(string).GetMethod(
                        nameof(string.Contains),
                        new[] { typeof(string) });
                    if (containsMethod != null)
                    {
                        filterExpression = Expression.Call(toStringCall, containsMethod, constant);
                    }
                    else
                    {
                        return query; // Skip unsupported type
                    }
                }
                else
                {
                    return query; // Skip unsupported type
                }
            }

            var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
            return (IMartenQueryable<T>)query.Where(lambda);
        }

        private static IMartenQueryable<T> ApplyMartenSearch<T>(
            IMartenQueryable<T> query,
            QueryParameters parameters,
            ColumnMap<T> columns)
        {
            if (string.IsNullOrWhiteSpace(parameters.Search))
                return query;

            var searchTerm = parameters.Search;
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var column in columns.Values)
            {
                Expression propertyAccess = column.Body;
                
                // Skip method calls (computed properties like string.Join) - they can't be queried in database
                if (propertyAccess is MethodCallExpression)
                    continue;
                
                if (propertyAccess is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                {
                    propertyAccess = unary.Operand;
                    // Check again after unwrapping
                    if (propertyAccess is MethodCallExpression)
                        continue;
                }

                // Replace parameter
                var visitor = new ParameterReplacer(column.Parameters[0], parameter);
                propertyAccess = visitor.Visit(propertyAccess);

                // Only search direct string properties (not computed)
                if (propertyAccess.Type == typeof(string) && propertyAccess is MemberExpression)
                {
                    // Use simple Contains - Marten will translate this to SQL ILIKE for case-insensitive search
                    var containsMethod = typeof(string).GetMethod(
                        nameof(string.Contains),
                        new[] { typeof(string) });
                    
                    if (containsMethod != null)
                    {
                        var constant = Expression.Constant(searchTerm, typeof(string));
                        var containsCall = Expression.Call(propertyAccess, containsMethod, constant);
                        combinedExpression = combinedExpression == null
                            ? containsCall
                            : Expression.OrElse(combinedExpression, containsCall);
                    }
                }
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                query = (IMartenQueryable<T>)query.Where(lambda);
            }

            return query;
        }

        private static IMartenQueryable<T> ApplyMartenSort<T>(
            IMartenQueryable<T> query,
            QueryParameters parameters,
            ColumnMap<T> columns)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
                return query;

            if (!columns.TryGet(parameters.SortBy, out var sortSelector))
                return query;

            return parameters.SortDesc
                ? (IMartenQueryable<T>)query.OrderByDescending(sortSelector)
                : (IMartenQueryable<T>)query.OrderBy(sortSelector);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;
            private readonly ParameterExpression _newParam;

            public ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;
                _newParam = newParam;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }

        /// <summary>
        /// Converts Marten IPagedList to standardized PagedResult
        /// </summary>
        public static PagedResult<T> ToPagedResult<T>(this IPagedList<T> pagedList)
        {
            return new PagedResult<T>
            {
                Items = pagedList.ToList(), // Convert IPagedList to IReadOnlyList
                Page = (int)pagedList.PageNumber,
                PageSize = (int)pagedList.PageSize,
                TotalCount = (int)pagedList.TotalItemCount
            };
        }
    }
}
