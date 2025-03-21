using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
  public class Category
  {
    public required int Id { get; set; }
    [Required, StringLength(100)]
    public required string Name { get; set; }

    public List<Product> Products { get; set; } = new List<Product>();
  }
}