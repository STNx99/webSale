using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public CategoryController(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return View(categories);
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id.ToString());
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id.ToString());
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id.ToString());
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Display(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id.ToString());
            if (category == null)
            {
                return NotFound();
            }
            category.Products = (await _productRepository.GetAllAsync()).Where(p => p.CategoryId == id).ToList();
            return View(category);
        }
    }
}
