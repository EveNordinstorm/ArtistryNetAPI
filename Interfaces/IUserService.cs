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
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        // Add other user-related methods as needed
    }
}
