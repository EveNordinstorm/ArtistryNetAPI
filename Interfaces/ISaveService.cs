using ArtistryNetAPI.Entities;

namespace ArtistryNetAPI.Interfaces
{
    public interface ISaveService
    {
        Task AddSaveAsync(Save save);
        Task RemoveSaveAsync(int postId, string userId);
        Task<IEnumerable<Save>> GetSavesForPostAsync(int postId);
        Task<List<Post>> GetUserSavedPostsAsync(int userId);
    }
}
