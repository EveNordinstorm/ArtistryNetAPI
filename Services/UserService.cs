using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using ArtistryNetAPI.Data;
using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace ArtistryNetAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, ApplicationDbContext context, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> RegisterUserAsync(RegisterModel model)
        {
            // Check if the username already exists
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                return new ObjectResult("UserName already exists.") { StatusCode = StatusCodes.Status400BadRequest };
            }

            string profilePhotoPath = string.Empty;

            // Handle profile photo upload
            if (model.ProfilePhoto != null)
            {
                _logger.LogInformation("Uploading profile photo for user: {UserName}", model.UserName);

                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images/profiles");
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfilePhoto.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePhoto.CopyToAsync(fileStream);
                }

                // Assign the unique file name to the profile photo path
                profilePhotoPath = uniqueFileName;
            }

            // Create a new instance of ApplicationUser
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                ProfilePhoto = profilePhotoPath,  // Store the unique file name in the user entity
                Bio = model.Bio
            };

            // Create the user
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("New user registered: {UserName}", model.UserName);
                return new ObjectResult("User registered successfully.") { StatusCode = StatusCodes.Status200OK };
            }

            // If registration failed, return a generic error message
            return new ObjectResult($"User registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}") { StatusCode = StatusCodes.Status400BadRequest };
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDto model)
        {
            _logger.LogInformation("Updating profile for user with ID: {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found for profile update: {UserId}", userId);
                return false;
            }

            _logger.LogInformation("Current UserName for user {UserId}: {UserName}", userId, user.UserName);

            if (model.ProfilePhoto != null)
            {
                _logger.LogInformation("Uploading new profile photo for user: {UserId}", userId);

                string profileUploadsFolder = Path.Combine(_environment.WebRootPath, "images/profiles");
                Directory.CreateDirectory(profileUploadsFolder);

                string profileFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfilePhoto.FileName);
                string profileFilePath = Path.Combine(profileUploadsFolder, profileFileName);

                using (var fileStream = new FileStream(profileFilePath, FileMode.Create))
                {
                    await model.ProfilePhoto.CopyToAsync(fileStream);
                }

                user.ProfilePhoto = profileFileName;
            }

            if (model.BannerPhoto != null)
            {
                _logger.LogInformation("Uploading new banner photo for user: {UserId}", userId);

                string bannerUploadsFolder = Path.Combine(_environment.WebRootPath, "images/banners");
                Directory.CreateDirectory(bannerUploadsFolder);

                string bannerFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.BannerPhoto.FileName);
                string bannerFilePath = Path.Combine(bannerUploadsFolder, bannerFileName);

                using (var fileStream = new FileStream(bannerFilePath, FileMode.Create))
                {
                    await model.BannerPhoto.CopyToAsync(fileStream);
                }

                user.BannerPhoto = bannerFileName;
            }
            else
            {
                user.BannerPhoto = null;
            }

            if (!string.IsNullOrWhiteSpace(model.Bio))
            {
                _logger.LogInformation("Updating bio for user: {UserId}", userId);
                user.Bio = model.Bio;
            }

            _logger.LogInformation("Attempting to save changes for user: {UserId}", userId);

            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                _logger.LogInformation("Profile update successful for user: {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Profile update failed for user: {UserId}. Errors: {Errors}",
                    userId, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }

            return updateResult.Succeeded;
        }


        public async Task<ApplicationUser> FindByUserNameAsync(string username)
        {
            _logger.LogInformation("Looking up user by username: {UserName}", username);
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser> FindByIdAsync(string id)
        {
            _logger.LogInformation("Looking up user by ID: {Id}", id);
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<(int followingCount, int followersCount)> GetFollowerCountsAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var followingCount = await _context.Followers.CountAsync(f => f.FollowerID == user.Id);
            var followersCount = await _context.Followers.CountAsync(f => f.FollowedID == user.Id);

            return (followingCount, followersCount);
        }
    }
}
