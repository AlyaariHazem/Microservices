using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BuildingBlocks.Querying.Models
{
    /// <summary>
    /// Model binder for QueryParameters to support query string binding
    /// Note: For complex filter scenarios, consider using query string parameters directly
    /// </summary>
    public class QueryParametersModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(QueryParameters))
            {
                return Task.CompletedTask;
            }

            var parameters = new QueryParameters
            {
                Search = GetValue(bindingContext, "search"),
                SortBy = GetValue(bindingContext, "sortBy"),
                SortDesc = GetBoolValue(bindingContext, "sortDesc"),
                Page = GetIntValue(bindingContext, "page", 1),
                PageSize = GetIntValue(bindingContext, "pageSize", 20),
                Filters = GetFilters(bindingContext)
            };

            bindingContext.Result = ModelBindingResult.Success(parameters);
            return Task.CompletedTask;
        }

        private static string? GetValue(ModelBindingContext context, string key)
        {
            var result = context.ValueProvider.GetValue(key);
            return result.FirstValue;
        }

        private static bool GetBoolValue(ModelBindingContext context, string key)
        {
            var value = GetValue(context, key);
            return bool.TryParse(value, out var result) && result;
        }

        private static int GetIntValue(ModelBindingContext context, string key, int defaultValue)
        {
            var value = GetValue(context, key);
            return int.TryParse(value, out var result) && result > 0 ? result : defaultValue;
        }

        private static Dictionary<string, string> GetFilters(ModelBindingContext context)
        {
            var filters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            // Simplified filter extraction - can be enhanced based on specific needs
            // For now, filters can be passed as query parameters like ?filterName=value
            // This can be extended to support filter[column]=value pattern if needed
            
            return filters;
        }
    }
}
