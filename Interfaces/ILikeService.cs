using ArtistryNetAPI.Entities;

namespace ArtistryNetAPI.Interfaces
{
    public interface ILikeService
    {
        Task AddLikeAsync(Like like);
        Task RemoveLikeAsync(int postId, string userId);
        Task<IEnumerable<Like>> GetLikesForPostAsync(int postId);
        Task<List<Post>> GetUserLikedPostsAsync(int userId);
    }
}
