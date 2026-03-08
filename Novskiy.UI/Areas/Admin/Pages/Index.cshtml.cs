using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Novskiy.Domain.Entities;
using Novskiy.UI.Services;

namespace Novskiy.UI.Areas.Admin.Pages;

[Authorize(Policy = "admin")]
public class IndexModel : PageModel
{
    private readonly IProductService _productService;

    public IndexModel(IProductService productService)
    {
        _productService = productService;
    }

    public List<Dish> Dish { get; set; } = default!;
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;

    public async Task OnGetAsync(int? pageNo = 1)
    {
        // Загружаем список через сервис
        var response = await _productService.GetProductListAsync(null, pageNo ?? 1);
        if (response.Success && response.Data != null)
        {
            Dish = response.Data.Items;
            CurrentPage = response.Data.CurrentPage;
            TotalPages = response.Data.TotalPages;
        }
    }
}
