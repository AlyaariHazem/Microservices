using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Querying.Models
{
    /// <summary>
    /// Standard paginated response model
    /// </summary>
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];

        public int Page { get; init; }

        public int PageSize { get; init; }

        public int TotalCount { get; init; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;

        public static async Task<PagedResult<T>> CreateAsync(
            IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken ct = default)
        {
            var count = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = count
            };
        }
    }
}
