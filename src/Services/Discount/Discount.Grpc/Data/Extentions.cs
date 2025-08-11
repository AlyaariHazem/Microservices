using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data
{
    public static class Extentions
    {
        public static IApplicationBuilder UseMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DiscountContext>();
            context.Database.MigrateAsync();
            return app;
        }
    }
}
