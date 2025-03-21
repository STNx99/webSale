using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;
using System.Threading.Tasks;
using System.Linq;


public class ProductController : Controller
{
  private readonly IProductRepository _productRepository;
  private readonly ICategoryRepository _categoryRepository;

  public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
  {
    _productRepository = productRepository;
    _categoryRepository = categoryRepository;
  }

  public async Task<IActionResult> AddProduct()
  {
    var categories = await _categoryRepository.GetAllCategoriesAsync();
    ViewBag.Categories = new SelectList(categories, "Id", "Name");
    return View();
  }

  public async Task<IActionResult> Index()
  {
    var products = await _productRepository.GetAllAsync();
    return View(products);
  }

  public async Task<IActionResult> Display(int id)
  {
    var product = await _productRepository.GetByIdAsync(id);
    if (product == null)
    {
      return NotFound();
    }
    return View(product);
  }

  public async Task<IActionResult> Update(int id)
  {
    var product = await _productRepository.GetByIdAsync(id);
    if (product == null)
    {
      return NotFound();
    }
    var categories = await _categoryRepository.GetAllCategoriesAsync();
    ViewBag.Categories = new SelectList(categories, "Id", "Name");
    return View(product);
  }

  [HttpPost]
  public async Task<IActionResult> Update(Product product)
  {
    if (ModelState.IsValid)
    {
      var existingProduct = await _productRepository.GetByIdAsync(product.Id);
      if (existingProduct == null)
      {
        return NotFound();
      }

      existingProduct.Name = product.Name;
      existingProduct.Price = product.Price;
      existingProduct.Description = product.Description;
      existingProduct.CategoryId = product.CategoryId;
      existingProduct.ImageUrl = product.ImageUrl;

      await _productRepository.UpdateAsync(existingProduct);
      return RedirectToAction("Index");
    }

    var categories = await _categoryRepository.GetAllCategoriesAsync();
    ViewBag.Categories = new SelectList(categories, "Id", "Name");
    return View(product);
  }

  public async Task<IActionResult> Delete(int id)
  {
    var product = await _productRepository.GetByIdAsync(id);
    if (product == null)
    {
      return NotFound();
    }
    return View(product);
  }

  [HttpPost, ActionName("Delete")]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    var product = await _productRepository.GetByIdAsync(id);
    if (product == null)
    {
      return NotFound();
    }
    await _productRepository.DeleteAsync(id);
    return RedirectToAction("Index");
  }

  [HttpPost]
  public async Task<IActionResult> AddProduct(Product product, IFormFile imageUrl)
  {
    if (ModelState.IsValid)
    {
      if (imageUrl != null)
      {
        product.ImageUrl = await SaveImage(imageUrl);
      }
      await _productRepository.AddAsync(product);
      return RedirectToAction("Index");
    }
    var categories = await _categoryRepository.GetAllCategoriesAsync();
    ViewBag.Categories = new SelectList(categories, "Id", "Name");
    return View(product);
  }

  private async Task<string> SaveImage(IFormFile imageFile)
  {
    var savePath = Path.Combine("wwwroot/images", imageFile.FileName);
    using (var fileStream = new FileStream(savePath, FileMode.Create))
    {
      await imageFile.CopyToAsync(fileStream);
    }
    return "/images/" + imageFile.FileName;
  }
}