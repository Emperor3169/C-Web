using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Novskiy.API.Controllers;
using Novskiy.API.Data;
using Novskiy.Domain.Entities;
using Novskiy.Domain.Models;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;

namespace Novskiy.Tests.Controllers;

public class DishesControllerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _contextOptions;
    private readonly IWebHostEnvironment _environment;

    public DishesControllerTests()
    {
        _environment = Substitute.For<IWebHostEnvironment>();

        // Создаем и открываем соединение. Это создает базу данных SQLite in-memory
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // Настройки для контекста
        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        // Создаем схему и заполняем данные
        using var context = new AppDbContext(_contextOptions);
        context.Database.EnsureCreated();

        var categories = new Category[]
        {
            new Category { Name = "Soups", NormalizedName = "soups" },
            new Category { Name = "Main Dishes", NormalizedName = "main-dishes" }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges();

        var sumCategory = context.Categories.First(c => c.NormalizedName == "soups");
        var mainCategory = context.Categories.First(c => c.NormalizedName == "main-dishes");

        var dishes = new List<Dish>
        {
            new Dish { Name = "Soup 1", CategoryId = sumCategory.Id, Calories = 100 },
            new Dish { Name = "Soup 2", CategoryId = sumCategory.Id, Calories = 200 },
            new Dish { Name = "Main 1", CategoryId = mainCategory.Id, Calories = 300 },
            new Dish { Name = "Main 2", CategoryId = mainCategory.Id, Calories = 400 },
            new Dish { Name = "Main 3", CategoryId = mainCategory.Id, Calories = 500 }
        };
        context.Dishes.AddRange(dishes);
        context.SaveChanges();
    }

    public void Dispose() => _connection?.Dispose();

    private AppDbContext CreateContext() => new AppDbContext(_contextOptions);

    // Проверка фильтра по категории
    [Fact]
    public async Task ControllerFiltersCategory()
    {
        // arrange
        using var context = CreateContext();
        var category = context.Categories.First();

        var controller = new DishesController(context, _environment);

        // act
        var response = await controller.GetDishes(category.NormalizedName, 1, 3);
        ResponseData<ListModel<Dish>>? responseData = response.Value;
        var dishesList = responseData?.Data?.Items;

        // assert
        Assert.NotNull(dishesList);
        Assert.True(dishesList.All(d => d.CategoryId == category.Id));
    }

    // Проверка подсчета количества страниц
    [Theory]
    [InlineData(2, 3)] // pageSize=2, всего объектов=5 -> 3 страницы
    [InlineData(3, 2)] // pageSize=3, всего объектов=5 -> 2 страницы
    public async Task ControllerReturnsCorrectPagesCount(int size, int qty)
    {
        // arrange
        using var context = CreateContext();
        var controller = new DishesController(context, _environment);

        // act
        var response = await controller.GetDishes(null, 1, size);
        ResponseData<ListModel<Dish>>? responseData = response.Value;
        
        // assert
        Assert.NotNull(responseData?.Data);
        var totalPages = responseData.Data.TotalPages;

        Assert.Equal(qty, totalPages);
    }

    // Проверка правильности возвращаемой страницы
    [Fact]
    public async Task ControllerReturnsCorrectPage()
    {
        // arrange
        using var context = CreateContext();
        var controller = new DishesController(context, _environment);
        
        // Первый объект на второй странице при размере страницы = 3, всего объектов 5
        Dish firstItemOnSecondPage = context.Dishes.Skip(3).First();

        // act - получаем 2-ю страницу
        var response = await controller.GetDishes(null, 2, 3);
        ResponseData<ListModel<Dish>>? responseData = response.Value;
        
        var dishesList = responseData?.Data?.Items;
        var currentPage = responseData?.Data?.CurrentPage;

        // assert
        Assert.NotNull(dishesList);
        Assert.Equal(2, currentPage);
        Assert.Equal(2, dishesList.Count); // На второй странице должно быть 2 объекта
        Assert.Equal(firstItemOnSecondPage.Id, dishesList[0].Id);
    }
}
