using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Novskiy.Domain.Entities;
using Novskiy.UI.Services;

namespace Novskiy.UI.Areas.Admin.Pages;

[Authorize(Policy = "admin")]
public class CreateModel : PageModel
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public CreateModel(ICategoryService categoryService, IProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var categoryListData = await _categoryService.GetCategoryListAsync();
        if (categoryListData.Success && categoryListData.Data != null)
        {
            ViewData["CategoryId"] = new SelectList(categoryListData.Data, "Id", "Name");
        }
        else
        {
            ViewData["CategoryId"] = new SelectList(new List<Category>(), "Id", "Name");
        }
        return Page();
    }

    [BindProperty]
    public Dish Dish { get; set; } = default!;

    [BindProperty]
    public IFormFile? Image { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(); // Перезагружаем категории для dropdown
            return Page();
        }

        // Вызов метода API через Service
        await _productService.CreateProductAsync(Dish, Image);
        
        return RedirectToPage("./Index");
    }
}
