
using Order.Application;
using Order.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Order.API.Infrastructure;
using Shared.Contracts;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Order.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty(
                    "Service",
                    "Order.API")
                .WriteTo.Console()
                .WriteTo.Seq("http://seq:80")
                .CreateLogger();

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

            builder.Services.AddSingleton<RabbitMqPublisher>();

            builder.Host.UseSerilog();

            builder.Services
            .AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://auth-api:8080";

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "MicroservicesDemo",

                        ValidateAudience = true,
                        ValidAudience =
                            "MicroservicesDemoUsers",

                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    "super_secret_key_12345"))
                    };
            });

            builder.Services.AddAuthorization();

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
                var publisher = app.Services.GetRequiredService<RabbitMqPublisher>();

                await publisher.PublishAsync(new OrderCreatedEvent
                {
                    //OrderId = dto.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    //UnitPrice = dto.UnitPrice,
                    //CreatedAt = dto.CreatedAt
                });

                //var order = await useCase.Execute(
                //    dto.ProductId,
                //    dto.Quantity);

                //return order is null
                //    ? Results.BadRequest("Invalid product")
                //    : Results.Ok(order);

                return Results.Ok();
            });

            app.MapGet("/orders", async (AppDbContext db) =>
            {
                return await db.Orders.ToListAsync();
            }).RequireAuthorization();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization(); 

            app.MapControllers();

            app.UseSerilogRequestLogging();
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.Run();
        }
    }
}
