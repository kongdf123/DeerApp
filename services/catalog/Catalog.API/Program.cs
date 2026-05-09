
using Catalog.API.Infrastructure;
using Catalog.Application;
using Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;

namespace Catalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // DB
            //builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=catalog.db"));
            builder.Services.AddDbContext<AppDbContext>(opt=>opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

            // Use cases
            builder.Services.AddScoped<Application.CreateProduct>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddHostedService<OrderCreatedConsumer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Endpoints
            app.MapGet("/products/{id:long}", async (
            AppDbContext db,
            long id) =>
            {
                var product = await db.Products.FindAsync(id);

                return product is null
                    ? Results.NotFound()
                    : Results.Ok(product);
            });
            app.MapGet("/products", async (AppDbContext db) => await db.Products.ToListAsync());

            app.MapPost("/products", async (CreateProduct useCase, ProductDto dto) =>
            {
                var product = await useCase.Execute(dto.Name, dto.Price);
                return Results.Created($"/products/{product.Id}", product);
            });

            app.MapControllers();

            app.Run();

        }
    }
}
