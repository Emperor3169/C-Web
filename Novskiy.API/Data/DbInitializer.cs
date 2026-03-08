using Microsoft.EntityFrameworkCore;
using Novskiy.Domain.Entities;

namespace Novskiy.API.Data;

public static class DbInitializer
{
    public static async Task SeedData(WebApplication app)
    {
        // Uri проекта
        var uri = "https://localhost:7002/";
        
        // Получение контекста БД
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Выполнение миграций
        await context.Database.MigrateAsync();

        // Заполнение данными
        if (!context.Categories.Any() && !context.Dishes.Any())
        {
            var categories = new Category[]
            {
                new Category { Name = "Стартеры", NormalizedName = "starters" },
                new Category { Name = "Салаты", NormalizedName = "salads" },
                new Category { Name = "Супы", NormalizedName = "soups"},
                new Category { Name = "Основные блюда", NormalizedName = "main-dishes"},
                new Category { Name = "Напитки", NormalizedName = "drinks" },
                new Category { Name = "Десерты", NormalizedName = "desserts" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            var dishes = new List<Dish>
            {
                new Dish { 
                    Name="Суп-харчо",
                    Description= "Очень острый, невкусный",
                    Calories = 200,
                    Category = categories.FirstOrDefault(c=>c.NormalizedName.Equals("soups")),
                    Image = uri + "Images/soup.jpg" 
                },
                new Dish { 
                    Name= "Борщ",
                    Description= "Много сала, без сметаны",
                    Calories = 330,
                    Category = categories.FirstOrDefault(c=>c.NormalizedName.Equals("soups")),
                    Image = uri + "Images/borsch.jpg" 
                },
                new Dish { 
                    Name="Котлета пожарская",
                    Description= "Хлеб - 80%, Морковь - 20%",
                    Calories = 635,
                    Category = categories.FirstOrDefault(c=>c.NormalizedName.Equals("main-dishes")),
                    Image = uri + "Images/cutlet.jpg" 
                },
                new Dish { 
                    Name = "Макароны по-флотски",
                    Description ="С охотничьей колбаской",
                    Calories = 524,
                    Category = categories.FirstOrDefault(c=>c.NormalizedName.Equals("main-dishes")),
                    Image = uri + "Images/pasta.jpg" 
                },
                new Dish { 
                    Name = "Компот",
                    Description= "Быстро растворимый, 2 литра",
                    Calories = 180,
                    Category = categories.FirstOrDefault(c=>c.NormalizedName.Equals("drinks")),
                    Image = uri + "Images/compote.jpg" 
                }
            };

            await context.AddRangeAsync(dishes);
            await context.SaveChangesAsync();
        }
    }
}
