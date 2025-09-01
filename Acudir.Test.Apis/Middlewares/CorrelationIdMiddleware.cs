using Serilog.Context;

namespace Acudir.Test.Apis.Middlewares
{
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            string cid = context.Request.Headers.TryGetValue(HeaderName, out Microsoft.Extensions.Primitives.StringValues val) && !string.IsNullOrWhiteSpace(val)
                ? val.ToString()
                : Guid.NewGuid().ToString();

            context.Items[HeaderName] = cid;
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(HeaderName))
                    context.Response.Headers.Add(HeaderName, cid);
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty("CorrelationId", cid))
            {
                await _next(context);
            }
        }
    }
}
