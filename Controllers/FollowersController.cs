using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ArtistryNetAPI.Utilities;
using ArtistryNetAPI.Data;
using ArtistryNetAPI.Services;

namespace ArtistryNetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowersController : ControllerBase
    {
        private readonly IFollowersService _followersService;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public FollowersController(IFollowersService followersService, IUserService userService, ApplicationDbContext context)
        {
            _followersService = followersService;
            _userService = userService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Follow([FromBody] FollowUserDto followUserDto)
        {
            var followerId = JwtHelper.GetUserIdFromToken(HttpContext);

            if (followerId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _followersService.FollowUserAsync(followerId, followUserDto.FollowedID);
            return Ok();
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckFollow([FromQuery] string followerId, [FromQuery] string followedId)
        {
            var exists = await _followersService.CheckIfFollowExists(followerId, followedId);
            return Ok(new { exists });
        }

        [HttpDelete("{followedId}")]
        public async Task<IActionResult> Unfollow(string followedId)
        {
            var followerId = JwtHelper.GetUserIdFromToken(HttpContext);

            if (followerId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = await _followersService.UnfollowUserAsync(followerId, followedId);

            if (result)
            {
                return Ok(new { message = "Unfollowed successfully." });
            }
            else
            {
                return NotFound(new { message = "User was not followed." });
            }
        }

        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetFollowers(string userId)
        {
            var followers = await _followersService.GetFollowersAsync(userId);
            return Ok(followers);
        }

        [HttpGet("{userId}/following")]
        public async Task<IActionResult> GetFollowing(string userId)
        {
            var following = await _followersService.GetFollowingAsync(userId);
            return Ok(following);
        }

        [HttpGet("{userId}/followersData")]
        public async Task<IActionResult> GetFollowersData(string userId)
        {
            var followers = await _followersService.GetFollowersDataAsync(userId);
            if (followers == null || followers.Count == 0)
            {
                return Ok(new List<UserAccountDto>());
            }

            var followersDetails = new List<UserAccountDto>();

            foreach (var followerId in followers)
            {
                var user = await _userService.FindByIdAsync(followerId);
                if (user != null)
                {
                    var profilePhotoUrl = Url.Content($"~/images/profiles/{Path.GetFileName(user.ProfilePhoto)}");
                    followersDetails.Add(new UserAccountDto
                    {
                        Id = user.Id,
                        Username = user.UserName,
                        ProfilePhoto = profilePhotoUrl
                    });
                }
            }

            return Ok(followersDetails);
        }

        [HttpGet("{userId}/followingData")]
        public async Task<IActionResult> GetFollowingData(string userId)
        {
            var following = await _followersService.GetFollowingDataAsync(userId);
            if (following == null || following.Count == 0)
            {
                return Ok(new List<UserAccountDto>());
            }

            var followingDetails = new List<UserAccountDto>();

            foreach (var followingId in following)
            {
                var user = await _userService.FindByIdAsync(followingId);
                if (user != null)
                {
                    var profilePhotoUrl = Url.Content($"~/images/profiles/{Path.GetFileName(user.ProfilePhoto)}");
                    followingDetails.Add(new UserAccountDto
                    {
                        Id = user.Id,
                        Username = user.UserName,
                        ProfilePhoto = profilePhotoUrl
                    });
                }
            }

            return Ok(followingDetails);
        }

        [HttpGet("{username}/counts")]
        public async Task<IActionResult> GetFollowerCounts(string username)
        {
            try
            {
                var (followingCount, followersCount) = await _userService.GetFollowerCountsAsync(username);
                return Ok(new { followingCount, followersCount });
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
}
