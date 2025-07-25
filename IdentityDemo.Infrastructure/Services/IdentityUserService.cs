﻿using IdentityDemo.Application.Dtos;
using IdentityDemo.Application.Users;
using IdentityDemo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace IdentityDemo.Infrastructure.Services;
public class IdentityUserService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole> roleManager) : IIdentityUserService
{
    public async Task<UserResultDto> CreateUserAsync(UserProfileDto user, string password)
    {
        const string roleName = "Administrator";

        var applicationUser = new ApplicationUser
        {
            UserName = user.Email,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
        var result = await userManager.CreateAsync(applicationUser, password);

        if (user.Email.Contains("admin"))
        {

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                // Skapa en ny roll
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // Lägg till en användare till en roll
            await userManager.AddToRoleAsync(applicationUser, roleName);
        }
        return new UserResultDto(result.Errors.FirstOrDefault()?.Description);
    }

    public async Task<UserProfileDto?> GetUserByIdAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return null;

        return new UserProfileDto(user.Email!, user.FirstName, user.LastName);
    }

    public async Task<UserResultDto> SignInAsync(string email, string password)
    {
        var result = await signInManager.PasswordSignInAsync(email, password, false, false);
        return new UserResultDto(result.Succeeded ? null : "Invalid user credentials");
    }

    public async Task SignOutAsync()
    {
        await signInManager.SignOutAsync();
    }
}
