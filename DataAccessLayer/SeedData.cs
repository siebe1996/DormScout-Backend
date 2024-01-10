using Globals.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new Backend_DormScoutContext(serviceProvider.GetRequiredService<DbContextOptions<Backend_DormScoutContext>>());
            using UserManager<User> _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            using RoleManager<Role> _roleManager = serviceProvider.GetService<RoleManager<Role>>();
            SeedRoles(_roleManager);
            SeedUsers(_userManager);
            /*SeedPlaces(context, _userManager);
            SeedReviews(context, _userManager);
            SeedAssessments(context, _userManager);*/
        }

        private static void SeedRoles(RoleManager<Role> _roleManager)
        {
            if (!_roleManager.RoleExistsAsync("User").Result)
            {
                _ = _roleManager.CreateAsync(new Role { Name = "User", Description = "Regular user role" }).Result;
            }

            if (!_roleManager.RoleExistsAsync("Admin").Result)
            {
                _ = _roleManager.CreateAsync(new Role { Name = "Admin", Description = "Administrator role" }).Result;
            }
        }

        private static void SeedUsers(UserManager<User> _userManager)
        {
            if (_userManager.FindByEmailAsync("admin@example.com").Result == null)
            {
                User adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    DateOfBirth = DateTime.Parse("1990-01-01"),
                    Email = "admin@example.com",
                    UserName = "admin",
                };
                _ = _userManager.CreateAsync(adminUser, "Azerty123!").Result;
                _ = _userManager.AddToRoleAsync(adminUser, "Admin").Result;
            }

            if (_userManager.FindByEmailAsync("user@example.com").Result == null)
            {
                User regularUser = new User
                {
                    FirstName = "Regular",
                    LastName = "User",
                    DateOfBirth = DateTime.Parse("1995-05-05"),
                    Email = "user@example.com",
                    UserName = "user",
                };
                _ = _userManager.CreateAsync(regularUser, "Azerty123!").Result;
                _ = _userManager.AddToRoleAsync(regularUser, "User").Result;
            }
        }

        /*private static void SeedPlaces(Backend_DormScoutContext context, UserManager<User> _userManager)
        {
            if (!context.Posts.Any())
            {
                User adminUser = _userManager.FindByNameAsync("admin").Result;
                if (adminUser != null)
                {
                    var posts = new[]
                    {
                        new Post
                        {
                            UserId = adminUser.Id,
                            Title = "Admin's Post 1",
                            Content = "This is the first post created by the admin user."
                        },
                        new Post
                        {
                            UserId = adminUser.Id,
                            Title = "Admin's Post 2",
                            Content = "This is the second post created by the admin user."
                        },
                    };

                    context.Posts.AddRange(posts);
                    context.SaveChanges();
                }
                else
                {
                    // Handle the scenario where the "admin" user is not found
                }
            }
        }*/
    }
}
