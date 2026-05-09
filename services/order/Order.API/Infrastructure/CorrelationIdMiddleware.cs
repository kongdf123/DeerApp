using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Order.Infrastructure
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context)
        {
            const string headerName =
                "X-Correlation-Id";

            if (!context.Request.Headers.TryGetValue(
                    headerName,
                    out var correlationId))
            {
                correlationId =
                    Guid.NewGuid().ToString();

                context.Request.Headers[headerName] =
                    correlationId;
            }

            context.Response.Headers[headerName] =
                correlationId;

            using (Serilog.Context.LogContext.PushProperty(
                       "CorrelationId",
                       correlationId.ToString()))
            {
                await _next(context);
            }
        }
    }
}
