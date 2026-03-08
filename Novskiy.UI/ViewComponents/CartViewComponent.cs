using Microsoft.AspNetCore.Mvc;
using Novskiy.Domain.Models;
using Novskiy.UI.Extensions;

namespace Novskiy.UI.ViewComponents;

public class CartViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var cart = HttpContext.Session.Get<Cart>("cart");
        return View(cart);
    }
}
