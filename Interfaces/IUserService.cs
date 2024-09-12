using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ArtistryNetAPI.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterModel model);
        Task<ApplicationUser> FindByUsernameAsync(string username);
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    }
}
