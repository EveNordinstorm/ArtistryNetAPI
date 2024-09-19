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
using ArtistryNetAPI.Dto;

[Route("api/[controller]")]
[ApiController]
public class LikesController : ControllerBase
{
    private readonly ILikeService _likeService;
    private readonly ApplicationDbContext _context;

    public LikesController(ILikeService likeService, ApplicationDbContext context)
    {
        _likeService = likeService;
        _context = context;
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

    [HttpGet("user")]
    public async Task<IActionResult> GetUserLikedPosts()
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var likedPosts = await _context.Likes
                .Include(l => l.Post)
                .ThenInclude(p => p.User)
                .Where(l => l.UserId == userIdFromToken)
                .Select(l => l.Post)
                .ToListAsync();

            var postDtos = likedPosts.Select(post => new PostDto
            {
                Id = post.Id,
                Username = post.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(post.User?.ProfilePhoto)}"),
                PostDateTime = post.PostDateTime,
                Description = post.Description,
                ImageUrl = Url.Content($"~/images/posts/{post.ImageUrl}"),
                UserId = post.UserId,
                IsLikedByUser = true
            });

            return Ok(postDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving liked posts: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving liked posts.");
        }
    }

}
