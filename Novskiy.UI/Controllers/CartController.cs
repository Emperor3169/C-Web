using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novskiy.Domain.Models;
using Novskiy.UI.Extensions;
using Novskiy.UI.Services;

namespace Novskiy.UI.Controllers;

public class CartController : Controller
{
    private readonly IProductService _productService;
    private Cart _cart = new();

    public CartController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: CartController
    public ActionResult Index()
    {
        _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
        return View(_cart.CartItems);
    }

    [Route("[controller]/add/{id:int}")]
    public async Task<ActionResult> Add(int id, string returnUrl)
    {
        var data = await _productService.GetProductByIdAsync(id);
        if (data.Success && data.Data != null)
        {
            _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            _cart.AddToCart(data.Data);
            HttpContext.Session.Set<Cart>("cart", _cart);
        }

        return Redirect(returnUrl ?? "/");
    }

    [Route("[controller]/remove/{id:int}")]
    public ActionResult Remove(int id)
    {
        _cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
        _cart.RemoveItems(id);
        HttpContext.Session.Set<Cart>("cart", _cart);
        return RedirectToAction("Index");
    }
}
