using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Data;
using ArtistryNetAPI.Dto;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ApplicationDbContext _context;

    public PostsController(IPostService postService, ApplicationDbContext context)
    {
        _postService = postService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromForm] PostModel model, [FromForm] IFormFile imageUrl)
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userIdFromToken);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var post = new Post
            {
                PostDateTime = DateTime.Now,
                Description = model.Description,
                UserId = userIdFromToken,
                Username = user.UserName,
                ProfilePhoto = user.ProfilePhoto
            };

            await _postService.CreatePostAsync(post, imageUrl, userIdFromToken);

            var imageUrlResult = Url.Content($"~/images/posts/{post.ImageUrl}");

            return Ok(new
            {
                Message = "Post created successfully",
                ImageUrl = imageUrlResult
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating post: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, "An error occurred while creating the post.");
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetPosts()
    {
        try
        {
            var posts = await _postService.GetAllPostsAsync();

            var postDtos = posts.Select(post => new PostDto
            {
                Id = post.Id,
                Username = post.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(post.User?.ProfilePhoto)}"),
                PostDateTime = post.PostDateTime,
                Description = post.Description,
                ImageUrl = Url.Content($"~/images/posts/{post.ImageUrl}"),
                UserId = post.UserId
            });

            return Ok(postDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving posts: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the posts.");
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserPosts()
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var posts = await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == userIdFromToken)
                .ToListAsync();

            var postDtos = posts.Select(post => new PostDto
            {
                Id = post.Id,
                Username = post.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(post.User?.ProfilePhoto)}"),
                PostDateTime = post.PostDateTime,
                Description = post.Description,
                ImageUrl = Url.Content($"~/images/posts/{post.ImageUrl}"),
                UserId = post.UserId
            });

            return Ok(postDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user posts: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, "An error occurred while retrieving the user's posts.");
        }
    }

}