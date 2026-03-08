using System.Text.Json;
using Novskiy.Domain.Entities;
using Novskiy.Domain.Models;

namespace Novskiy.UI.Services;

public class ApiProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ApiProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
    {
        var uri = _httpClient.BaseAddress?.ToString() ?? "";

        var queryData = new Dictionary<string, string>();
        queryData.Add("pageNo", pageNo.ToString());
        
        if (!string.IsNullOrEmpty(categoryNormalizedName))
        {
            queryData.Add("category", categoryNormalizedName);
        }

        var query = QueryString.Create(queryData!);
        var result = await _httpClient.GetAsync(uri + query.Value);
        
        if (result.IsSuccessStatusCode)
        {
            var data = await result.Content.ReadFromJsonAsync<ResponseData<ListModel<Dish>>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (data != null) return data;
        }

        var response = new ResponseData<ListModel<Dish>>
        {
            Success = false,
            ErrorMessage = "Ошибка чтения API"
        };
        return response;
    }
    
    // Заглушка для получения конкретного товара по ID (пока не реализовано в API)
    public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }

    public Task DeleteProductAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
    {
        var serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Подготовить объект, возвращаемый методом
        var responseData = new ResponseData<Dish>();

        // Послать запрос к API для сохранения объекта
        var response = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress, product);

        if (!response.IsSuccessStatusCode)
        {
            responseData.Success = false;
            responseData.ErrorMessage = $"Не удалось создать объект: {response.StatusCode}";
            return responseData;
        }

        // Если файл изображения передан клиентом
        if (formFile != null)
        {
            // получить созданный объект из ответа API-сервиса
            var dish = await response.Content.ReadFromJsonAsync<Dish>(serializerOptions);
            
            if (dish == null)
            {
                responseData.Success = false;
                responseData.ErrorMessage = "Ошибка десериализации созданного объекта";
                return responseData;
            }

            // создать объект запроса
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_httpClient.BaseAddress?.AbsoluteUri}{dish.Id}")
            };

            // Создать контент типа multipart/form-data
            var content = new MultipartFormDataContent();
            // создать потоковый контент из переданного файла
            var streamContent = new StreamContent(formFile.OpenReadStream());
            // добавить потоковый контент в общий контент под именем "image" (очень важно совпадение имени переменной в контроллере)
            content.Add(streamContent, "image", formFile.FileName);
            // поместить контент в запрос
            request.Content = content;
            // послать запрос к Api-сервису
            response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                responseData.Success = false;
                responseData.ErrorMessage = $"Не удалось сохранить изображение: {response.StatusCode}";
                return responseData;
            }
        }
        
        return responseData;
    }
}
