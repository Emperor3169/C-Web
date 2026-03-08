using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novskiy.API.Data;
using Novskiy.Domain.Entities;
using Novskiy.Domain.Models;

namespace Novskiy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Categories
    [HttpGet]
    public async Task<ActionResult<ResponseData<IEnumerable<Category>>>> GetCategories()
    {
        var response = new ResponseData<IEnumerable<Category>>
        {
            Data = await _context.Categories.ToListAsync()
        };
        return response;
    }
}
