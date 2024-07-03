using KrizzShop_DataAccess.Initializer;

namespace KrizzShop.Middleware
{
    public class DbInitializeMiddleware
    {
        private readonly RequestDelegate _next;

        public DbInitializeMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, IDbInitializer dbInitializer)
        {
            dbInitializer.Initialize();
            await _next.Invoke(context);
        }
    }
}
