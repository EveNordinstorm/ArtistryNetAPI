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

        public async Task<IEnumerable<Save>> GetSavesForPostAsync(int postId)
        {
            return await _context.Saves
                .Where(s => s.PostId == postId)
                .ToListAsync();
        }
    }
}
