using ArtistryNetAPI.Entities;

namespace ArtistryNetAPI.Interfaces
{
    public interface ISaveService
    {
        Task AddSaveAsync(Save save);
        Task<IEnumerable<Save>> GetSavesForPostAsync(int postId);
    }
}
