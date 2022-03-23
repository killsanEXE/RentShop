using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager)
        {
            if(await userManager.Users.AnyAsync()) return;

            List<AppUser> users = new() {
                new() { UserName="lisa", Name="lisa", Email="lisa@gmail.com", EmailConfirmed=true},
                new() { UserName="todd", Name="todd", Email="todd@gmail.com", EmailConfirmed=true},
                new() { UserName="eliot", Name="eliot", Email="eliot@gmail.com", EmailConfirmed=true},
                new() { UserName="leon", Name="leon", Email="leon@gmail.com", EmailConfirmed=true},
                new() { UserName="max", Name="max", Email="max@gmail.com", EmailConfirmed=true},
            };

            var roles = new List<AppRole>
            {
                new AppRole { Name = "Client" },
                new AppRole { Name = "Admin" },
                new AppRole { Name = "Deliveryman" },
            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users){
                user.UserName = user.UserName!.ToLower();
                user.PhotoUrl = "https://res.cloudinary.com/killsan/image/upload/v1647499317/oi55f6gb6tkzxpjxokk4.jpg";
                user.DateOfBirth = DateTime.Parse("1985-01-23");
                await userManager.CreateAsync(user, "Perehod2020");
                await userManager.AddToRoleAsync(user, "Client");
            }

            var admin = new AppUser
            {
                UserName = "admin",
                DateOfBirth = DateTime.Parse("1000-01-23")
            };

            await userManager.CreateAsync(admin, "Perehod2020");
            await userManager.AddToRolesAsync(admin, new[] {"Admin"});
        }
    }
}