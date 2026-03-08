using Novskiy.Domain.Entities;

namespace Novskiy.Domain.Models;

public class CartItem
{
    public Dish Item { get; set; } = default!;
    public int Qty { get; set; }
}
