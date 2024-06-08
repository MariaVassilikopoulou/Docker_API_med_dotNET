using API_med_dotNET.Models;
using API_med_dotNET.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace API_med_dotNET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load configuration
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();

            var server = Environment.GetEnvironmentVariable("DBServer") ?? "mssql-server";
            var port = Environment.GetEnvironmentVariable("DBPort") ?? "1433";
            var user = Environment.GetEnvironmentVariable("DBUser") ?? "SA";
            var password = Environment.GetEnvironmentVariable("DBPassword") ?? "Pa55word2024";
            var database = Environment.GetEnvironmentVariable("DBDatabaseName") ?? "MyDocker";

            Console.WriteLine($"DBServer: {server}");
            Console.WriteLine($"DBPort: {port}");
            Console.WriteLine($"DBUser: {user}");
            Console.WriteLine($"DBPassword: {password}");
            Console.WriteLine($"DBDatabaseName: {database}");

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database))
            {
                Console.WriteLine("One or more environment variables are not set.");
                Console.WriteLine($"DBServer: {server}, DBPort: {port}, DBUser: {user}, DBPassword: {password}, DBDatabaseName: {database}");
                throw new InvalidOperationException("Database configuration is incomplete.");
            }

            var connectionString = $"Server={server},{port};Database={database};User ID={user};Password={password};TrustServerCertificate=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite";

            Console.WriteLine($"Connection String: {connectionString}");

            // Add services to the container
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(); // Enable retry logic for transient errors
                }).EnableSensitiveDataLogging()); // Log sensitive data to help diagnose issues

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
            });

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    policyBuilder =>
                    {
                        policyBuilder.AllowAnyOrigin()
                                     .AllowAnyHeader()
                                     .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            try
            {
                DatabaseManagementService.MigrationInitialisation(app);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return;
            }

            // Configure the HTTP request pipeline
            app.UseCors("AllowAllOrigins");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_med_dotNet");
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapGet("/products", async (DataContext context) =>
                await context.Products.ToListAsync());

            app.MapGet("/product/{id}", async (DataContext context, int id) => await context.Products.FindAsync(id) is Products product ?
                Results.Ok(product) :
                Results.NotFound("Sorry no Product with that Id!")
            );

            app.MapPost("/product", async (DataContext context, Products product) =>
            {
                context.Products.Add(product);
                await context.SaveChangesAsync();
                return Results.Ok(await context.Products.ToListAsync());
            });

            app.MapPut("/product/{id}", async (DataContext context, int id, Products product) =>
            {
                var productToUpdate = await context.Products.FindAsync(id);
                if (productToUpdate == null)
                {
                    return Results.NotFound("No product to Update with that Id!");
                }

                productToUpdate.ProductName = product.ProductName;
                productToUpdate.ProductPrice = product.ProductPrice;
                productToUpdate.ProductType = product.ProductType;
                await context.SaveChangesAsync();
                return Results.Ok(await context.Products.ToListAsync());
            });

            app.MapDelete("/product/{id}", async (DataContext context, int id) =>
            {
                var productToDelete = await context.Products.FindAsync(id);
                if (productToDelete == null)
                {
                    return Results.NotFound("There is no product to Delete with that Id!");
                }
                context.Products.Remove(productToDelete);
                await context.SaveChangesAsync();
                return Results.Ok(await context.Products.ToListAsync());
            });

            app.Run();
        }
    }
}

