using BuildingBlocks.Querying.Models;
using BuildingBlocks.Querying.Security;

namespace BuildingBlocks.Querying.Abstractions
{
    /// <summary>
    /// Abstraction for applying query parameters to data sources
    /// </summary>
    public interface IQueryApplier<T>
    {
        IQueryable<T> Apply(IQueryable<T> query, QueryParameters parameters, ColumnMap<T> columns);
    }
}
