
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.API
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

            var jwtSettings = builder.Configuration.GetSection("Jwt");

            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            builder.Services
                .AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer =
                                jwtSettings["Issuer"],

                            ValidAudience =
                                jwtSettings["Audience"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(key)
                        };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
             
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapPost("/login", (AuthLoginRequest request) =>
            {
                // demo-only hardcoded auth
                if (request.Username != "admin" ||
                    request.Password != "password")
                {
                    return Results.Unauthorized();
                }

                var claims = new[]
                {
                    new Claim(
                        ClaimTypes.Name,
                        request.Username),

                    new Claim(
                        ClaimTypes.Role,
                        "Admin")
                };

                var key =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings["Key"]!));

                var creds =
                    new SigningCredentials(
                        key,
                        SecurityAlgorithms.HmacSha256);

                var token =
                    new JwtSecurityToken(
                        issuer: jwtSettings["Issuer"],
                        audience: jwtSettings["Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(1),
                        signingCredentials: creds);

                var jwt =
                    new JwtSecurityTokenHandler()
                        .WriteToken(token);

                return Results.Ok(new
                {
                    access_token = jwt
                });
            });

            app.Run();
        }
    }
}
