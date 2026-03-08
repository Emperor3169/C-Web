using Microsoft.AspNetCore.Mvc;
using Novskiy.Domain.Entities;
using Novskiy.Domain.Models;
using Novskiy.UI.Controllers;
using Novskiy.UI.Services;
using NSubstitute;

namespace Novskiy.Tests.Controllers;

public class ProductControllerTests
{
    private IProductService _productService = default!;
    private ICategoryService _categoryService = default!;

    public ProductControllerTests()
    {
        SetupData();
    }

    // Список категорий сохраняется во ViewData
    [Fact]
    public async Task IndexPutsCategoriesToViewData()
    {
        // arrange
        var controller = new ProductController(_categoryService, _productService);

        // act
        var response = await controller.Index(null);

        // assert
        var view = Assert.IsType<ViewResult>(response);
        var categories = Assert.IsAssignableFrom<IEnumerable<Category>>(view.ViewData["categories"]);
        Assert.Equal(6, categories.Count());
        Assert.Equal("Все", view.ViewData["currentCategory"]);
    }

    // Имя текущей категории сохраняется во ViewData
    [Fact]
    public async Task IndexSetsCorrectCurrentCategory()
    {
        // arrange
        var categories = await _categoryService.GetCategoryListAsync();
        var currentCategory = categories.Data!.First();
        var controller = new ProductController(_categoryService, _productService);

        // act
        var response = await controller.Index(currentCategory.NormalizedName);

        // assert
        var view = Assert.IsType<ViewResult>(response);
        Assert.Equal(currentCategory.Name, view.ViewData["currentCategory"]);
    }

    // В случае ошибки возвращается NotFoundObjectResult
    [Fact]
    public async Task IndexReturnsNotFound()
    {
        // arrange
        string errorMessage = "Test error";
        var categoriesResponse = new ResponseData<List<Category>>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
        
        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoriesResponse));
        var controller = new ProductController(_categoryService, _productService);

        // act
        var response = await controller.Index(null);

        // assert
        var result = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(errorMessage, result.Value?.ToString());
    }

    // Настройка имитации ICategoryService и IProductService
    private void SetupData()
    {
        _categoryService = Substitute.For<ICategoryService>();
        var categoriesResponse = new ResponseData<List<Category>>
        {
            Data = new List<Category>
            {
                new Category { Id = 1, Name = "Стартеры", NormalizedName = "starters" },
                new Category { Id = 2, Name = "Салаты", NormalizedName = "salads" },
                new Category { Id = 3, Name = "Супы", NormalizedName = "soups" },
                new Category { Id = 4, Name = "Основные блюда", NormalizedName = "main-dishes" },
                new Category { Id = 5, Name = "Напитки", NormalizedName = "drinks" },
                new Category { Id = 6, Name = "Десерты", NormalizedName = "desserts" }
            }
        };

        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoriesResponse));

        _productService = Substitute.For<IProductService>();

        var dishes = new List<Dish>
        {
            new Dish { Id = 1, Name = "Dish 1" },
            new Dish { Id = 2, Name = "Dish 2" },
            new Dish { Id = 3, Name = "Dish 3" },
            new Dish { Id = 4, Name = "Dish 4" },
            new Dish { Id = 5, Name = "Dish 5" }
        };

        var productResponse = new ResponseData<ListModel<Dish>>
        {
            Data = new ListModel<Dish> { Items = dishes, CurrentPage = 1, TotalPages = 1 }
        };

        _productService.GetProductListAsync(Arg.Any<string?>(), Arg.Any<int>())
            .Returns(Task.FromResult(productResponse));
    }
}
