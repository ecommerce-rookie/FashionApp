using Duende.IdentityModel;
using EcommerceApp.IdentityService.Data;
using EcommerceApp.IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace EcommerceApp.IdentityService
{
    public class SeedData
    {
        public static void EnsureSeedData(WebApplication app)
        {
            using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                // Migrate database
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                // Create roles if they do not exist
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = new string[] { "Admin", "Tester", "User" };

                foreach (var role in roles)
                {
                    if (!roleManager.RoleExistsAsync(role).Result)
                    {
                        var roleResult = roleManager.CreateAsync(new IdentityRole(role)).Result;
                        if (!roleResult.Succeeded)
                        {
                            throw new Exception(roleResult.Errors.First().Description);
                        }
                        Log.Debug($"{role} role created");
                    } else
                    {
                        Log.Debug($"{role} role already exists");
                    }
                }

                // Get UserManager to handle user creation and claims/role assignments.
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Seed 'alice' account
                var alice = userMgr.FindByNameAsync("alice").Result;
                if (alice == null)
                {
                    alice = new ApplicationUser
                    {
                        UserName = "alice",
                        Email = "AliceSmith@email.com",
                        EmailConfirmed = true,
                    };
                    var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    // Add claims to alice
                    result = userMgr.AddClaimsAsync(alice, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Id, alice.Id),
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    // Assign a role to alice (e.g., Admin)
                    result = userMgr.AddToRoleAsync(alice, "Admin").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("alice created and assigned Admin role");
                } else
                {
                    Log.Debug("alice already exists");
                }

                // Seed 'bob' account
                var bob = userMgr.FindByNameAsync("bob").Result;
                if (bob == null)
                {
                    bob = new ApplicationUser
                    {
                        UserName = "bob",
                        Email = "BobSmith@email.com",
                        EmailConfirmed = true
                    };
                    var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    // Add claims to bob
                    result = userMgr.AddClaimsAsync(bob, new Claim[]
                    {
                new Claim(JwtClaimTypes.Name, "Bob Smith"),
                new Claim(JwtClaimTypes.GivenName, "Bob"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                new Claim("location", "somewhere")
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    // Assign a role to bob (e.g., Tester)
                    result = userMgr.AddToRoleAsync(bob, "Tester").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("bob created and assigned Tester role");
                } else
                {
                    Log.Debug("bob already exists");
                }
            }
        }

    }
}
