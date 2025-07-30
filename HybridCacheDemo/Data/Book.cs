using System.ComponentModel.DataAnnotations;

namespace HybridCacheDemo.Data;

public class Book
{
    public int Id { get; init; }
    
    [StringLength(250), Required]
    public required string Title { get; init; }
    
    [StringLength(100), Required]
    public required string Author { get; init; }
    
    [StringLength(100), Required]
    public required string Genre { get; init; }
    public required int Year { get; set; }
}
