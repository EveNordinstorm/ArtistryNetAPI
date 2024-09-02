using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Utilities;
using ArtistryNetAPI.Models;
using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class LikesController : ControllerBase
{
    private readonly ILikeService _likeService;

    public LikesController(ILikeService likeService)
    {
        _likeService = likeService;
    }

    [HttpPost]
    public async Task<IActionResult> AddLike([FromBody] LikeModel model)
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var like = new Like
            {
                PostId = model.PostId,
                UserId = userIdFromToken,
                LikeDateTime = DateTime.UtcNow
            };

            await _likeService.AddLikeAsync(like);

            return Ok(new { message = "Like added successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding like: {ex.Message}");
            return StatusCode(500, "An error occurred while adding the like.");
        }
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> RemoveLike(int postId)
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _likeService.RemoveLikeAsync(postId, userIdFromToken);

            return Ok(new { message = "Like removed successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing like: {ex.Message}");
            return StatusCode(500, "An error occurred while removing the like.");
        }
    }

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetLikesForPost(int postId)
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var likes = await _likeService.GetLikesForPostAsync(postId);
            var isLikedByUser = likes.Any(like => like.UserId == userIdFromToken);

            return Ok(new { isLikedByUser });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving likes: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the likes.");
        }
    }
}
