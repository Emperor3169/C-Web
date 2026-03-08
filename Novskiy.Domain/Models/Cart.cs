using Novskiy.Domain.Entities;

namespace Novskiy.Domain.Models;

public class Cart
{
    public int Id { get; set; }
    
    /// <summary>
    /// Список объектов в корзине
    /// key - идентификатор объекта
    /// </summary>
    public Dictionary<int, CartItem> CartItems { get; set; } = new();

    /// <summary>
    /// Добавить объект в корзину
    /// </summary>
    /// <param name="dish">Добавляемый объект</param>
    public virtual void AddToCart(Dish dish)
    {
        if (CartItems.ContainsKey(dish.Id))
        {
            CartItems[dish.Id].Qty++;
        }
        else
        {
            CartItems.Add(dish.Id, new CartItem
            {
                Item = dish,
                Qty = 1
            });
        }
    }

    /// <summary>
    /// Удалить объект из корзины
    /// </summary>
    /// <param name="id">Идентификатор удаляемого объекта</param>
    public virtual void RemoveItems(int id)
    {
        CartItems.Remove(id);
    }

    /// <summary>
    /// Очистить корзину
    /// </summary>
    public virtual void ClearAll()
    {
        CartItems.Clear();
    }

    /// <summary>
    /// Количество объектов в корзине
    /// </summary>
    public int Count { get => CartItems.Sum(item => item.Value.Qty); }

    /// <summary>
    /// Общее количество калорий
    /// </summary>
    public double TotalCalories 
    {
        get => CartItems.Sum(item => item.Value.Item.Calories * item.Value.Qty);
    }
}
