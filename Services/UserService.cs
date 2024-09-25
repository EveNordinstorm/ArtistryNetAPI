using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using ArtistryNetAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ArtistryNetAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
        {
            // Handle file upload
            string profilePhotoPath = string.Empty;
            if (model.ProfilePhoto != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images/profiles");
                Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                profilePhotoPath = Path.Combine(uploadsFolder, model.ProfilePhoto.FileName);
                using (var fileStream = new FileStream(profilePhotoPath, FileMode.Create))
                {
                    await model.ProfilePhoto.CopyToAsync(fileStream);
                }
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                ProfilePhoto = profilePhotoPath,
                Bio = model.Bio
            };

            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<ApplicationUser> FindByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
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
