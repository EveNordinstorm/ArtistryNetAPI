using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Services;
using ArtistryNetAPI.Interfaces;
using System.IdentityModel.Tokens.Jwt;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    private string GetUserIdFromToken()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        if (authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid"); // or "sub" depending on your token
            return userIdClaim?.Value;
        }

        return null;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromForm] PostModel model)
    {
        var userId = GetUserIdFromToken();
        if (model.UserId != userId)
        {
            return Unauthorized(new { Message = "Invalid user" });
        }

        var post = new Post
        {
            Username = model.Username,
            ProfilePhoto = model.ProfilePhoto,
            PostDateTime = DateTime.Now,
            Description = model.Description,
            UserId = model.UserId
        };

        await _postService.CreatePostAsync(post, model.ImagePath);

        var imageUrl = Url.Content("~/images/posts/" + model.ImagePath.FileName);

        return Ok(new
        {
            Message = "Post created successfully",
            ImageUrl = imageUrl // Return the image URL
        });
    }

    // Can add Get, Update, Delete, etc. using the _postService
}
