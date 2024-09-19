using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Data;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;

namespace ArtistryNetAPI.Services
{
    public class SaveService : ISaveService
    {
        private readonly ApplicationDbContext _context;

        public SaveService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddSaveAsync(Save save)
        {
            _context.Saves.Add(save);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveSaveAsync(int postId, string userId)
        {
            var save = await _context.Saves.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
            if (save != null)
            {
                _context.Saves.Remove(save);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Save>> GetSavesForPostAsync(int postId)
        {
            return await _context.Saves
                .Where(s => s.PostId == postId)
                .ToListAsync();
        }

        public async Task<List<Post>> GetUserSavedPostsAsync(int userId)
        {
            return await _context.Saves
                .Where(s => s.UserId == userId.ToString())
                .Select(s => s.Post)
                .ToListAsync();
        }
    }
}
