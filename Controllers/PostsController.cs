﻿using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> CreatePost([FromForm] PostModel model, [FromForm] IFormFile? imageUrl)
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
                UserName = user.UserName,
                ProfilePhoto = user.ProfilePhoto
            };

            await _postService.CreatePostAsync(post, imageUrl, userIdFromToken);

            string? imageUrlResult = null;
            if (post.ImageUrl != null)
            {
                imageUrlResult = Url.Content($"~/images/posts/{post.ImageUrl}");
            }

            return Ok(new
            {
                Id = post.Id,
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = await _postService.DeletePostAsync(id, userIdFromToken);

            if (result == null)
            {
                return NotFound(new { message = "Post not found." });
            }

            if (!result.Value)
            {
                return Forbid("You are not authorized to delete this post.");
            }

            return Ok(new { message = "Post deleted successfully." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting post: {ex.Message}");
            return StatusCode(500, "An error occurred while deleting the post.");
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
                UserName = post.User?.UserName,
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound();

            var postDto = new PostDto
            {
                Id = post.Id,
                UserName = post.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(post.User?.ProfilePhoto)}"),
                PostDateTime = post.PostDateTime,
                Description = post.Description,
                ImageUrl = Url.Content($"~/images/posts/{post.ImageUrl}"),
                UserId = post.UserId
            };

            return Ok(postDto);
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
                UserName = post.User?.UserName,
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

    [HttpGet("getPostsByUserName/{username}")]
    public async Task<IActionResult> GetPostsByUserNameAsync(string username)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var posts = await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            var postDtos = posts.Select(post => new PostDto
            {
                Id = post.Id,
                UserName = post.User?.UserName,
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
            Console.WriteLine($"Error retrieving posts by username: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the posts.");
        }
    }
}