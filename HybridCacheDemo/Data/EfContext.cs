using Microsoft.EntityFrameworkCore;

namespace HybridCacheDemo.Data;

public class EfContext(DbContextOptions<EfContext> options)  : DbContext(options)
{
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "The Fellowship of the Ring", Author = "J.R.R. Tolkien", Genre = "Fantasy", Year = 1954 },
            new Book { Id = 2, Title = "The Two Towers", Author = "J.R.R. Tolkien", Genre = "Fantasy", Year = 1954 },
            new Book { Id = 3, Title = "The Return of the King", Author = "J.R.R. Tolkien", Genre = "Fantasy", Year = 1955 },
        
            new Book { Id = 4, Title = "Dune", Author = "Frank Herbert", Genre = "Science Fiction", Year = 1965 },
            new Book { Id = 5, Title = "Ender's Game", Author = "Orson Scott Card", Genre = "Science Fiction", Year = 1985 }
        );
    }
}
