using Novskiy.Domain.Entities;
using Novskiy.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Novskiy.UI.Services;

public class MemoryProductService : IProductService
{
    private List<Dish> _dishes = default!;
    private List<Category> _categories = default!;
    private readonly IConfiguration _config;

    public MemoryProductService(ICategoryService categoryService, IConfiguration config)
    {
        _categories = categoryService.GetCategoryListAsync().Result.Data;
        _config = config;
        SetupData();
    }

    /// <summary>
    /// Инициализация списков
    /// </summary>
    private void SetupData()
    {
        _dishes = new List<Dish>
        {
            new Dish { 
                Id = 1, 
                Name = "Суп-харчо", 
                Description = "Острый грузинский суп с говядиной и рисом", 
                Calories = 250, 
                Image = "Images/soup.jpg", 
                CategoryId = _categories.Find(c => c.NormalizedName.Equals("soups"))!.Id 
            },
            new Dish { 
                Id = 2, 
                Name = "Борщ", 
                Description = "Традиционный свекольник со сметаной и мясом", 
                Calories = 330, 
                Image = "Images/borsch.jpg", 
                CategoryId = _categories.Find(c => c.NormalizedName.Equals("soups"))!.Id 
            },
            new Dish { 
                Id = 3, 
                Name = "Цезарь с курицей", 
                Description = "Салат с куриной грудкой, пармезаном и сухариками", 
                Calories = 400, 
                Image = "Images/cezar.jpg", 
                CategoryId = _categories.Find(c => c.NormalizedName.Equals("salads"))!.Id 
            },
            new Dish { 
                Id = 4, 
                Name = "Стейк из говядины", 
                Description = "Сочный стейк прожарки medium с овощами гриль", 
                Calories = 650, 
                Image = "Images/steak.jpg", 
                CategoryId = _categories.Find(c => c.NormalizedName.Equals("main-courses"))!.Id 
            },
            new Dish { 
                Id = 5, 
                Name = "Чизкейк Нью-Йорк", 
                Description = "Классический творожный десерт", 
                Calories = 420, 
                Image = "Images/cheesecake.jpg", 
                CategoryId = _categories.Find(c => c.NormalizedName.Equals("desserts"))!.Id 
            }
        };
    }

    public Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
    {
        // Создать объект результата
        var result = new ResponseData<ListModel<Dish>>();

        // Id категории для фильтрации
        int? categoryId = null;

        if (categoryNormalizedName != null)
        {
            categoryId = _categories.Find(c => c.NormalizedName.Equals(categoryNormalizedName))?.Id;
        }

        // Выбрать объекты, отфильтрованные по Id категории
        var data = _dishes.Where(d => categoryId == null || d.CategoryId.Equals(categoryId)).ToList();

        // получить размер страницы из конфигурации
        int pageSize = _config.GetSection("ItemsPerPage").Get<int>();
        // получить общее количество страниц
        int totalPages = (int)Math.Ceiling(data.Count / (double)pageSize);

        // получить данные страницы
        var listData = new ListModel<Dish>()
        {
            Items = data.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList(),
            CurrentPage = pageNo,
            TotalPages = totalPages
        };

        // Поместить данные в объект результата
        result.Data = listData;

        // Если список пустой
        if (data.Count == 0)
        {
            result.Success = false;
            result.ErrorMessage = "Нет объектов в выбранной категории";
        }

        return Task.FromResult(result);
    }

    public Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }

    public Task DeleteProductAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }
}
