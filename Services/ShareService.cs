using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Data;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtistryNetAPI.Services
{
    public class ShareService : IShareService
    {
        private readonly ApplicationDbContext _context;

        public ShareService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddShareAsync(Share share)
        {
            _context.Shares.Add(share);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveShareAsync(int postId, string userId)
        {
            var share = await _context.Shares.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
            if (share != null)
            {
                _context.Shares.Remove(share);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Share>> GetSharesForPostAsync(int postId)
        {
            return await _context.Shares
                .Where(s => s.PostId == postId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Share>> GetSharesByUserAsync(string userId)
        {
            return await _context.Shares
                .Where(s => s.UserId == userId)
                .Include(s => s.User)
                .Include(s => s.Post)
                    .ThenInclude(p => p.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Share>> GetAllSharesAsync()
        {
            return await _context.Shares
                .Include(s => s.User)
                .Include(s => s.Post)
                    .ThenInclude(p => p.User)
                .ToListAsync();
        }
    }
}
