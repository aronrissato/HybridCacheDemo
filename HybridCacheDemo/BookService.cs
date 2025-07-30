using System.Net.Mime;
using System.Text.Json;
using HybridCacheDemo.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

namespace HybridCacheDemo;

public class BookService(
    EfContext _context,
    IDistributedCache distributedCache,
    HybridCache hybridCache,
    IMemoryCache memoryCache)
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<Book?> GetBookAsync(int id)
    {
        var key = $"book-{id}";
        return await hybridCache.GetOrCreateAsync(key,
            async (cancellation) => 
                await _context.Books.FindAsync(id));

        // option 3
        // var key = $"book-{id}";
        // var cachedBook = await _distributedCache.GetAsync(key);
        // if (cachedBook is not null)
        //     return JsonSerializer.Deserialize<Book>(cachedBook);
        //
        // var book = await _context.Books.FindAsync(id);
        // if (book is null)
        //     return null;
        //
        // await _distributedCache.SetAsync(key,
        //     JsonSerializer.SerializeToUtf8Bytes(book));
        // return book;

        // option 2
        // var key = $"book-{id}";
        // return await _memoryCache.GetOrCreate(key, async (entry) =>
        // {
        //     await Task.Delay(2000);
        //     return await _context.Books.FindAsync(id);
        // })!;

        // option 1
        // var key = $"book-{id}";
        // if (_memoryCache.TryGetValue(key, out Book? book))
        // {
        //     return book;
        // }
        //
        // await Task.Delay(2000);
        // book = await context.Books.FindAsync(id);
        // _memoryCache.Set(key, book);
        // return book;
    }

    public async Task<Book?> UpdateYearAsync(int id, int year)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return null;

        book.Year = year;
        await _context.SaveChangesAsync();

        await hybridCache.SetAsync($"book-{id}", book);

        return book;
    }
}