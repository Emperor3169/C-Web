using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Novskiy.Domain.Entities;
using Novskiy.UI.Services;

namespace Novskiy.UI.Areas.Admin.Pages;

[Authorize(Policy = "admin")]
public class DetailsModel : PageModel
{
    private readonly IProductService _productService;

    public DetailsModel(IProductService productService)
    {
        _productService = productService;
    }

    public Dish Dish { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var response = await _productService.GetProductByIdAsync(id.Value);
        if (!response.Success || response.Data == null) return NotFound();

        Dish = response.Data;
        return Page();
    }
}
