
using Order.Application;
using Order.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Order.API.Infrastructure;

namespace Order.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres")));

            builder.Services.AddScoped<CreateOrder>();

            builder.Services
                .AddHttpClient<CatalogApiClient>(client =>
                {
                    //client.BaseAddress = new Uri("http://localhost:7001");
                    client.BaseAddress = new Uri("http://catalog-api:8080");
                })
                .AddPolicyHandler(HttpPolicies.RetryPolicy())
                .AddPolicyHandler(HttpPolicies.TimeoutPolicy())
                .AddPolicyHandler(HttpPolicies.CircuitBreakerPolicy());

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Configuration.AddEnvironmentVariables();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.MapPost("/orders", async (
            CreateOrder useCase,
            CreateOrderDto dto) =>
            {
                var order = await useCase.Execute(
                    dto.ProductId,
                    dto.Quantity);

                return order is null
                    ? Results.BadRequest("Invalid product")
                    : Results.Ok(order);
            });

            app.MapGet("/orders", async (AppDbContext db) =>
            {
                return await db.Orders.ToListAsync();
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
