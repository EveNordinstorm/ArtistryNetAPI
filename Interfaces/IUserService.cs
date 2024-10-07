using ArtistryNetAPI.Dto;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ArtistryNetAPI.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> RegisterUserAsync(RegisterModel model);
        Task<ApplicationUser> FindByUserNameAsync(string username);
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDto model);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<(int followingCount, int followersCount)> GetFollowerCountsAsync(string username);
    }
}
