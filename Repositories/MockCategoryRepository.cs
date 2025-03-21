using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Models;
using WebsiteBanHang.DataAccess; 

namespace WebsiteBanHang.Repositories
{
  public class MockCategoryRepository : ICategoryRepository
  {
    private readonly ApplicationDbContext _context;

    public MockCategoryRepository(ApplicationDbContext _context)
    {
      this._context = _context;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
      return await _context.Categories.ToListAsync();
    }

    public async Task<Category> GetByIdAsync(string id)
    {
      return await _context.Categories.FindAsync(int.Parse(id));
    }

    public async Task AddAsync(Category category)
    {
      await _context.Categories.AddAsync(category);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
      _context.Categories.Update(category);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
      var category = await _context.Categories.FindAsync(int.Parse(id));
      if (category != null)
      {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
      }
    }
  }
}