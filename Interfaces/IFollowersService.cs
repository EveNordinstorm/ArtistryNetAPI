namespace ArtistryNetAPI.Interfaces
{
    public interface IFollowersService
    {
        Task FollowUserAsync(string followerId, string followedId);
        Task<bool> CheckIfFollowExists(string followerId, string followedId);
        Task<bool> UnfollowUserAsync(string followerId, string followedId);
        Task<IEnumerable<string>> GetFollowersAsync(string userId);
        Task<IEnumerable<string>> GetFollowingAsync(string userId);
        Task<List<string>> GetFollowersDataAsync(string userId);
        Task<List<string>> GetFollowingDataAsync(string userId);
        Task<int> GetFollowersCountAsync(string userId);
        Task<int> GetFollowingCountAsync(string userId);
    }
}
