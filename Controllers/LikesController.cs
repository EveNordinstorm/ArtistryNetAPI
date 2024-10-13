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
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> RemoveLike(int postId)
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _likeService.RemoveLikeAsync(postId, userIdFromToken);

            return Ok(new { message = "Like removed successfully" });
        }
    }

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetLikesForPost(int postId)
    {
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
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserLikedPosts()
    {
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
                UserName = post.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(post.User?.ProfilePhoto)}"),
                PostDateTime = post.PostDateTime,
                Description = post.Description,
                ImageUrl = Url.Content($"~/images/posts/{post.ImageUrl}"),
                UserId = post.UserId,
                IsLikedByUser = true
            });

            return Ok(postDtos);
        }
    }

}
