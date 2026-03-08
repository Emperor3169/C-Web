using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Novskiy.UI.Data;

public class DbInit
{
    public static async Task SeedData(WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var userManager = scope
            .ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync("admin@gmail.com");
        if (user == null)
        {
            user = new ApplicationUser();
            await userManager.SetEmailAsync(user, "admin@gmail.com");
            await userManager.SetUserNameAsync(user, "admin@gmail.com");
            user.EmailConfirmed = true;

            await userManager.CreateAsync(user, "123456");

            var claim = new Claim(ClaimTypes.Role, "admin");
            await userManager.AddClaimAsync(user, claim);
        }
    }
}
