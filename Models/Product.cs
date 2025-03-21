using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanHang.Models
{
    public class Product
    {
        public required int Id { get; set; }
        [Required, StringLength(100)]
        public required string Name { get; set; }
        [Range(0.01, 1000)]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        
        [ForeignKey("Category")]
        public int CategoryId { get; set; } 
        public Category? Category { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }

    public class ProductImage{
        public required int Id { get; set; }
        public required string? Url {get; set;}

        public required int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}