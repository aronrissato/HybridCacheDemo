using HybridCacheDemo;
using HybridCacheDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .WriteTo.Console(theme: AnsiConsoleTheme.Code)
             .CreateLogger();

// Use Serilog as the logging provider
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddDbContext<EfContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<BookService>();

builder.Services.AddMemoryCache(options => { 
    options.ExpirationScanFrequency = TimeSpan.FromSeconds(15);
});

builder.Services.AddHybridCache();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.MapGet("/books/{id:int}", (int id, BookService service) => service.GetBookAsync(id));

app.MapGet("/genre/{genre}/books", async (string genre, EfContext context, HybridCache cache) =>
{
    string[] tags = [$"genre-{genre}", "books"];

    return await cache.GetOrCreateAsync(
        $"genre-{genre}-books", async (cancellation) =>
        {
            var books = await context.Books.Where(x => x.Genre == genre).ToListAsync();
            await Task.Delay(1000, cancellation);
            return books;
        },
        tags: tags
    );
});

app.MapPost("/genre/{genre}/invalidate", async (string genre, HybridCache cache) =>
{
    await cache.RemoveByTagAsync($"genre-{genre}");
    return Results.Ok("Invalidated!");
});

app.MapPut("/books/{id:int}", async (int id, [FromBody] UpdateBookRequest request, BookService service) =>
{
    var book = await service.UpdateYearAsync(id, request.Year);
    return Results.Ok(book);
});

await app.RunAsync();

public record UpdateBookRequest(int Year);