using System.Text.Json.Serialization;

namespace Novskiy.Domain.Entities;

public class Dish
{
    public int Id { get; set; } // id блюда
    public string Name { get; set; } = string.Empty; // название блюда
    public string Description { get; set; } = string.Empty; // описание блюда
    public int Calories { get; set; } // калории
    public string? Image { get; set; } // путь к файлу изображения

    // Навигационные свойства
    /// <summary>
    /// группа блюд (например, супы, салаты и т.д.)
    /// </summary>
    public int CategoryId { get; set; }
    
    [JsonIgnore]
    public Category? Category { get; set; }
}
