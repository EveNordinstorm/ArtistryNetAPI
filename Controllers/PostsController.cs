using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Utilities;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
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

            if (userIdFromToken != model.UserId)
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var post = new Post
            {
                Username = model.Username,
                ProfilePhoto = model.ProfilePhoto,
                PostDateTime = DateTime.Now,
                Description = model.Description
            };

            await _postService.CreatePostAsync(post, imageUrl);

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
            return StatusCode(500, "An error occurred while creating the post.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts()
    {
        try
        {
            var posts = await _postService.GetAllPostsAsync();

            var postDtos = posts.Select(post => new
            {
                post.Id,
                post.Username,
                post.ProfilePhoto,
                post.PostDateTime,
                post.Description,
                ImageUrl = Url.Content($"~/images/posts/{post.ImageUrl}")
            });

            return Ok(postDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving posts: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the posts.");
        }
    }
}
