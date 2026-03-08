using Novskiy.Domain.Entities;
using Novskiy.Domain.Models;

namespace Novskiy.UI.Services;

public interface ICategoryService
{
    /// <summary>
    /// Получение списка всех категорий
    /// </summary>
    /// <returns></returns>
    public Task<ResponseData<List<Category>>> GetCategoryListAsync();
}
