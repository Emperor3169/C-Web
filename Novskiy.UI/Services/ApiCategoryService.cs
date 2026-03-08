using Novskiy.Domain.Entities;
using Novskiy.Domain.Models;

namespace Novskiy.UI.Services;

public class ApiCategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;

    public ApiCategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
    {
        var result = await _httpClient.GetAsync(_httpClient.BaseAddress);
        if (result.IsSuccessStatusCode)
        {
            var data = await result.Content.ReadFromJsonAsync<ResponseData<List<Category>>>();
            if (data != null) return data;
        }

        var response = new ResponseData<List<Category>>
        {
            Success = false,
            ErrorMessage = "Ошибка чтения API"
        };
        return response;
    }
}
