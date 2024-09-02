using ArtistryNetAPI.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArtistryNetAPI.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post> GetPostByIdAsync(int id);
        Task CreatePostAsync(Post post, IFormFile image, string userId);
        Task UpdatePostAsync(Post post);
        Task DeletePostAsync(int id);
    }
}
