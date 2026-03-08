using Microsoft.AspNetCore.Mvc;
using Novskiy.UI.Services;

namespace Novskiy.UI.Controllers;

public class ProductController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public ProductController(ICategoryService categoryService, IProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }

    [Route("Catalog")]
    [Route("Catalog/{category}")]
    public async Task<IActionResult> Index(string? category, int pageNo = 1)
    {
        // получить список категорий
        var categoriesResponse = await _categoryService.GetCategoryListAsync();

        // если список не получен, вернуть код 404
        if (!categoriesResponse.Success)
            return NotFound(categoriesResponse.ErrorMessage);

        // передать список категорий во ViewData
        ViewData["categories"] = categoriesResponse.Data;

        // передать во ViewData имя текущей категории
        var currentCategory = category == null
            ? "Все"
            : categoriesResponse.Data.FirstOrDefault(c => c.NormalizedName == category)?.Name;
        ViewData["currentCategory"] = currentCategory;

        // получить отфильтрованный список продуктов
        var productResponse = await _productService.GetProductListAsync(category, pageNo);
        if (!productResponse.Success)
            ViewData["Error"] = productResponse.ErrorMessage;

        return View(productResponse.Data?.Items ?? new List<Novskiy.Domain.Entities.Dish>());
    }
}
