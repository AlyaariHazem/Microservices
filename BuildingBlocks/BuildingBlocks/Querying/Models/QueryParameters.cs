namespace BuildingBlocks.Querying.Models
{
    /// <summary>
    /// Unified query parameters for filtering, sorting, searching, and pagination
    /// </summary>
    public sealed record QueryParameters
    {
        /// <summary>
        /// Search query string for full-text search
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Column -> Value filters
        /// </summary>
        public Dictionary<string, string> Filters { get; init; } = new();

        /// <summary>
        /// Column name to sort by
        /// </summary>
        public string? SortBy { get; init; }    

        /// <summary>
        /// Sort in descending order
        /// </summary>
        public bool SortDesc { get; init; }

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; init; } = 20;
    }
}
