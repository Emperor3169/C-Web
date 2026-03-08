using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novskiy.API.Data;
using Novskiy.Domain.Entities;
using Novskiy.Domain.Models;

namespace Novskiy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DishesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public DishesController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: api/Dishes
    [HttpGet]
    public async Task<ActionResult<ResponseData<ListModel<Dish>>>> GetDishes(
        string? category, 
        int pageNo = 1, 
        int pageSize = 3)
    {
        // Создать объект результата
        var result = new ResponseData<ListModel<Dish>>();

        // Фильтрация по категории загрузка данных категории
        var data = _context.Dishes
            .Include(d => d.Category)
            .Where(d => string.IsNullOrEmpty(category)
                    || d.Category!.NormalizedName.Equals(category));
            
        // Подсчет общего количества страниц
        int totalPages = (int)Math.Ceiling(data.Count() / (double)pageSize);

        if(pageNo > totalPages && totalPages > 0)
            pageNo = totalPages;

        // Создание объекта ListModel с нужной страницей данных
        var listData = new ListModel<Dish>()
        {
            Items = await data
                            .Skip((pageNo - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync(),
            CurrentPage = pageNo,
            TotalPages = totalPages
        };
        
        // Поместить данные в объект результата
        result.Data = listData;

        // Если список пустой
        if (!data.Any())
        {
            result.Success = false;
            result.ErrorMessage = "Нет объектов в выбранной категории";
        }

        return result;
    }

    // POST: api/Dishes/{id}
    [HttpPost("{id}")]
    public async Task<IActionResult> SaveImage(int id, IFormFile image)
    {
        // Найти объект по Id
        var dish = await _context.Dishes.FindAsync(id);
        if (dish == null)
        {
            return NotFound();
        }

        // Путь к папке wwwroot/Images
        var imagesPath = Path.Combine(_env.WebRootPath, "Images");
        // получить случайное имя файла
        var randomName = Path.GetRandomFileName();
        // получить расширение исходного файла
        var extension = Path.GetExtension(image.FileName);
        // задать новому имени расширение
        var fileName = Path.ChangeExtension(randomName, extension);
        // полный путь к файлу
        var filePath = Path.Combine(imagesPath, fileName);
        
        // создать файл и открыть поток для записи
        using var stream = System.IO.File.OpenWrite(filePath);
        // скопировать файл в поток
        await image.CopyToAsync(stream);
        
        // получить Url хоста
        var host = "https://" + Request.Host;
        // Url файла изображения
        var url = $"{host}/Images/{fileName}";
        // Сохранить url файла в объекте
        dish.Image = url;
        
        await _context.SaveChangesAsync();
        return Ok();
    }
}
