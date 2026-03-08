using Novskiy.Domain.Entities;
using Novskiy.Domain.Models;

namespace Novskiy.Blazor.Services;

public class ApiProductService(HttpClient Http) : IProductService<Dish>
{
    private List<Dish> _dishes = default!;
    private int _currentPage = 1;
    private int _totalPages = 1;

    public IEnumerable<Dish> Products => _dishes;
    public int CurrentPage => _currentPage;
    public int TotalPages => _totalPages;

    public event Action? ListChanged;

    public async Task GetProducts(int pageNo, int pageSize)
    {
        // Url сервиса API
        string uri = Http.BaseAddress!.AbsoluteUri;

        // данные для Query запроса
        var queryData = new Dictionary<string, string>
        {
            { "pageNo", pageNo.ToString() },
            { "pageSize", pageSize.ToString() }
        };

        var query = QueryString.Create(queryData!);

        // Отправить запрос http
        var result = await Http.GetAsync(uri + query.Value);

        // В случае успешного ответа
        if (result.IsSuccessStatusCode)
        {
            // получить данные из ответа
            var responseData = await result.Content
                .ReadFromJsonAsync<ResponseData<ListModel<Dish>>>();

            // обновить параметры
            if (responseData?.Data != null)
            {
                _currentPage = responseData.Data.CurrentPage;
                _totalPages = responseData.Data.TotalPages;
                _dishes = responseData.Data.Items;
                ListChanged?.Invoke();
            }
        }
        // В случае ошибки
        else
        {
            _dishes = new List<Dish>();
            _currentPage = 1;
            _totalPages = 1;
        }
    }
}
