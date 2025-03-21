using System.Collections.Generic;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Repositories
{
  public interface ICategoryRepository{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetByIdAsync(string id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(string id);
    
  }
}
