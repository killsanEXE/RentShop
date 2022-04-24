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

            List<AppUser> clients = new() 
            {
                new() { UserName="lisa", Name="lisa", Email="lisa@gmail.com", EmailConfirmed=true},
                new() { UserName="todd", Name="todd", Email="todd@gmail.com", EmailConfirmed=true},
                new() { UserName="leon", Name="leon", Email="leon@gmail.com", EmailConfirmed=true},
            };

            List<AppUser> deliverymans = new()
            {
                new() { UserName="eliot", Name="eliot", Email="eliot@gmail.com", EmailConfirmed=true},
                new() { UserName="max", Name="max", Email="max@gmail.com", EmailConfirmed=true},
            };

            var roles = new List<AppRole>
            {
                new AppRole { Name = "Client" },
                new AppRole { Name = "Admin" },
                new AppRole { Name = "Deliveryman" },
            };

            foreach(var role in roles) await roleManager.CreateAsync(role);

            foreach(var user in clients)
            {
                user.UserName = user.UserName!.ToLower();
                user.DateOfBirth = DateTime.Parse("1985-01-23");
                await userManager.CreateAsync(user, "Perehod2020");
                await userManager.AddToRoleAsync(user, "Client");
            }

            foreach(var user in deliverymans)
            {
                user.UserName = user.UserName!.ToLower();
                user.Location = new() 
                {
                    Country = "Belarus"
                };
                user.DateOfBirth = DateTime.Parse("1985-01-23");
                await userManager.CreateAsync(user, "Perehod2020");
                await userManager.AddToRoleAsync(user, "Client");
                await userManager.AddToRoleAsync(user, "Deliveryman");
            }

            var admin = new AppUser
            {
                UserName = "admin",
                DateOfBirth = DateTime.Parse("1000-01-23"),
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, "Perehod2020");
            await userManager.AddToRolesAsync(admin, new[] {"Admin"});
        }
    }
}