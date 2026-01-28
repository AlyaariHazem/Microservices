using System.Linq.Expressions;
using BuildingBlocks.Querying.Models;
using BuildingBlocks.Querying.Security;

namespace BuildingBlocks.Querying.Extensions
{
    /// <summary>
    /// Core query engine for applying filters, sorting, and searching
    /// Note: This is a simplified version. For complex scenarios, consider using IQueryApplier implementations
    /// </summary>
    public static class QueryableExtensions
    {
        public static IQueryable<T> Apply<T>(
            this IQueryable<T> query,
            QueryParameters parameters,
            ColumnMap<T> columns)
        {
            // Filtering
            foreach (var filter in parameters.Filters)
            {
                if (!columns.TryGet(filter.Key, out var selector))
                    continue;

                query = ApplyFilter(query, selector, filter.Value);
            }

            // Searching (if Search is provided)
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                query = ApplySearch(query, parameters.Search, columns);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                if (columns.TryGet(parameters.SortBy, out var sortSelector))
                {
                    query = parameters.SortDesc
                        ? query.OrderByDescending(sortSelector)
                        : query.OrderBy(sortSelector);
                }
            }

            return query;
        }

        private static IQueryable<T> ApplyFilter<T>(
            IQueryable<T> query,
            Expression<Func<T, object>> selector,
            string value)
        {
            // For Marten compatibility, we need to handle the selector differently
            // This is a simplified version - for production, consider a more robust implementation
            var parameter = Expression.Parameter(typeof(T), "x");
            
            // Unwrap the object conversion if present
            Expression propertyAccess;
            if (selector.Body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                propertyAccess = unary.Operand;
            }
            else
            {
                propertyAccess = selector.Body;
            }

            // Replace the parameter
            var visitor = new ParameterReplacer(selector.Parameters[0], parameter);
            propertyAccess = visitor.Visit(propertyAccess);

            // Handle string contains
            if (propertyAccess.Type == typeof(string))
            {
                var constant = Expression.Constant(value, typeof(string));
                var containsMethod = typeof(string).GetMethod(
                    nameof(string.Contains),
                    new[] { typeof(string) });
                var containsCall = Expression.Call(propertyAccess, containsMethod!, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);
                return query.Where(lambda);
            }

            // For other types, try equality
            var valueConstant = Expression.Constant(Convert.ChangeType(value, propertyAccess.Type));
            var equals = Expression.Equal(propertyAccess, valueConstant);
            var equalsLambda = Expression.Lambda<Func<T, bool>>(equals, parameter);
            return query.Where(equalsLambda);
        }

        private static IQueryable<T> ApplySearch<T>(
            IQueryable<T> query,
            string searchTerm,
            ColumnMap<T> columns)
        {
            var searchableColumns = columns.Values.ToList();
            if (searchableColumns.Count == 0)
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var column in searchableColumns)
            {
                Expression propertyAccess = column.Body;
                if (propertyAccess is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                {
                    propertyAccess = unary.Operand;
                }

                // Replace parameter
                var visitor = new ParameterReplacer(column.Parameters[0], parameter);
                propertyAccess = visitor.Visit(propertyAccess);

                if (propertyAccess.Type == typeof(string))
                {
                    var constant = Expression.Constant(searchTerm, typeof(string));
                    var containsMethod = typeof(string).GetMethod(
                        nameof(string.Contains),
                        new[] { typeof(string) });
                    var containsCall = Expression.Call(propertyAccess, containsMethod!, constant);
                    
                    combinedExpression = combinedExpression == null
                        ? containsCall
                        : Expression.OrElse(combinedExpression, containsCall);
                }
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
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
    }
}
