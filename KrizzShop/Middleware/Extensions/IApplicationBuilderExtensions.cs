namespace KrizzShop.Middleware.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder) 
            => builder.UseMiddleware<DbInitializeMiddleware>();
    }
}
