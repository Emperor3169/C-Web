using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Novskiy.UI.Data;

namespace Novskiy.UI.Controllers;

public class ImageController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public ImageController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _env = env;
    }

    public async Task<IActionResult> GetAvatar()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && user.Avatar != null && user.Avatar.Length > 0)
                {
                    return File(user.Avatar, user.MimeType);
                }
            }
        }

        // Если не аутентифицирован или нет аватара - отдаем заглушку
        var imagePath = Path.Combine(_env.WebRootPath, "images", "default-profile-picture.png");
        return PhysicalFile(imagePath, "image/png");
    }
}
