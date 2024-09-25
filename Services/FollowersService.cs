using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Data;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;

namespace ArtistryNetAPI.Services
{
    public class FollowersService : IFollowersService
    {
        private readonly ApplicationDbContext _context;

        public FollowersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task FollowUserAsync(string followerId, string followedId)
        {
            var followerExists = await _context.Users.AnyAsync(u => u.Id == followerId);
            var followedExists = await _context.Users.AnyAsync(u => u.Id == followedId);

            if (!followerExists || !followedExists)
            {
                throw new ArgumentException("Follower or followed user not found.");
            }

            var follower = new Follower
            {
                FollowerID = followerId,
                FollowedID = followedId
            };

            _context.Followers.Add(follower);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfFollowExists(string followerId, string followedId)
        {
            return await _context.Followers
                .AnyAsync(f => f.FollowerID == followerId && f.FollowedID == followedId);
        }


        public async Task<bool> UnfollowUserAsync(string followerId, string followedId)
        {
            var follower = await _context.Followers
                .FirstOrDefaultAsync(f => f.FollowerID == followerId && f.FollowedID == followedId);

            if (follower != null)
            {
                _context.Followers.Remove(follower);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<string>> GetFollowersAsync(string userId)
        {
            return await _context.Followers
                .Where(f => f.FollowedID == userId)
                .Select(f => f.FollowerID)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetFollowingAsync(string userId)
        {
            return await _context.Followers
                .Where(f => f.FollowerID == userId)
                .Select(f => f.FollowedID)
                .ToListAsync();
        }

        public async Task<int> GetFollowersCountAsync(string userId)
        {
            return await _context.Followers.CountAsync(f => f.FollowedID == userId);
        }

        public async Task<int> GetFollowingCountAsync(string userId)
        {
            return await _context.Followers.CountAsync(f => f.FollowerID == userId);
        }

        public async Task<List<string>> GetFollowersDataAsync(string userId)
        {
            var followers = await _context.Followers
                .Where(f => f.FollowedID == userId)
                .Select(f => f.FollowerID)
                .ToListAsync();

            return followers;
        }

        public async Task<List<string>> GetFollowingDataAsync(string userId)
        {
            var following = await _context.Followers
                .Where(f => f.FollowerID == userId)
                .Select(f => f.FollowedID)
                .ToListAsync();

            return following;
        }
    }
}
