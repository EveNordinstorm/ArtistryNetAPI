using ArtistryNetAPI.Entities;

namespace ArtistryNetAPI.Interfaces
{
    public interface IShareService
    {
        Task AddShareAsync(Share share);
        Task RemoveShareAsync(int postId, string userId);
        Task<IEnumerable<Share>> GetSharesForPostAsync(int postId);
        Task<IEnumerable<Share>> GetSharesByUserAsync(string UserId);
    }
}
