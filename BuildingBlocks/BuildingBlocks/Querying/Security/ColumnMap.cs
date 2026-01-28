using System.Linq.Expressions;

namespace BuildingBlocks.Querying.Security
{
    /// <summary>
    /// Maps column names to property selectors for secure query building
    /// </summary>
    public sealed class ColumnMap<T>
    {
        private readonly Dictionary<string, Expression<Func<T, object>>> _map;

        public ColumnMap()
        {
            _map = new Dictionary<string, Expression<Func<T, object>>>(StringComparer.OrdinalIgnoreCase);
        }

        public ColumnMap<T> Add(string name, Expression<Func<T, object>> selector)
        {
            _map[name] = selector;
            return this;
        }

        public bool TryGet(string name, out Expression<Func<T, object>> selector)
        {
            return _map.TryGetValue(name, out selector!);
        }

        public IEnumerable<Expression<Func<T, object>>> Values => _map.Values;
    }
}
