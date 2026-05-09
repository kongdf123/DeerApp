using Polly.Extensions.Http;
using Polly;

namespace Order.API.Infrastructure
{
    public class HttpPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> RetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine(
                            $"Retry {retryAttempt} after {timespan.TotalSeconds}s");
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                5);
        }

        public static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    3,
                    TimeSpan.FromSeconds(15));
        }
    }
}
