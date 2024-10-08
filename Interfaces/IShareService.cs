﻿using ArtistryNetAPI.Entities;

namespace ArtistryNetAPI.Interfaces
{
    public interface IShareService
    {
        Task AddShareAsync(Share share);
        Task RemoveShareAsync(int postId, string userId);
        Task<IEnumerable<Share>> GetSharesByUserNameAsync(string username);
        Task<IEnumerable<Share>> GetSharesForPostAsync(int postId);
        Task<Share> GetShareByIdAsync(int id);
        Task<bool> HasUserSharedPostAsync(string userId, int postId);
        Task<IEnumerable<Share>> GetSharesByUserAsync(string UserId);
        Task<IEnumerable<Share>> GetAllSharesAsync();
    }
}
