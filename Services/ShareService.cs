using ArtistryNetAPI.Data;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            var share = await _context.Shares
                .FirstOrDefaultAsync(s => s.PostId == postId && s.UserId == userId);

            if (share != null)
            {
                _context.Shares.Remove(share);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Share>> GetSharesByUserNameAsync(string userName)
        {
            return await _context.Shares
                .Include(s => s.User)
                .Include(s => s.Post)
                .Where(s => s.User.UserName == userName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Share>> GetSharesForPostAsync(int postId)
        {
            return await _context.Shares
                .Include(s => s.User)
                .Include(s => s.Post)
                .Where(s => s.PostId == postId)
                .ToListAsync();
        }

        public async Task<Share> GetShareByIdAsync(int id)
        {
            return await _context.Shares
                .Include(s => s.User)
                .Include(s => s.Post)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> HasUserSharedPostAsync(string userId, int postId)
        {
            return await _context.Shares
                .AnyAsync(s => s.UserId == userId && s.PostId == postId);
        }

        public async Task<IEnumerable<Share>> GetSharesByUserAsync(string userId)
        {
            return await _context.Shares
                .Include(s => s.User)
                .Include(s => s.Post)
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Share>> GetAllSharesAsync()
        {
            return await _context.Shares
                .Include(s => s.User)
                .Include(s => s.Post)
                .ToListAsync();
        }
    }
}
