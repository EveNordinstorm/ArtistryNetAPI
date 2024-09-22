﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Utilities;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Services;
using ArtistryNetAPI.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class SharesController : ControllerBase
{
    private readonly IShareService _shareService;
    private readonly ApplicationDbContext _context;

    public SharesController(IShareService shareService, ApplicationDbContext context)
    {
        _shareService = shareService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddShare([FromBody] ShareModel model)
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var share = new Share
            {
                PostId = model.PostId,
                UserId = userIdFromToken,
                ShareDateTime = DateTime.Now
            };

            await _shareService.AddShareAsync(share);

            return Ok(new { id = share.Id, message = "Share added successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding share: {ex.Message}");
            return StatusCode(500, "An error occurred while adding the share.");
        }
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetSharesForPost(int postId)
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var shares = await _shareService.GetSharesForPostAsync(postId);
            var isSharedByUser = shares.Any(share => share.UserId == userIdFromToken);

            return Ok(new { isSharedByUser });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving shares: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the shares.");
        }
    }

    [HttpGet("share-status/{postId}")]
    public async Task<IActionResult> CheckShareStatusAsync(int postId)
    {
        var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

        if (userIdFromToken == null)
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        var isShared = await _shareService.HasUserSharedPostAsync(userIdFromToken, postId);
        return Ok(new { isSharedByUser = isShared });
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> RemoveShare(int postId)
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _shareService.RemoveShareAsync(postId, userIdFromToken);

            return Ok(new { message = "Share removed successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing share: {ex.Message}");
            return StatusCode(500, "An error occurred while removing the share.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShares()
    {
        try
        {
            var shares = await _shareService.GetAllSharesAsync();

            var sharesDto = shares.Select(share => new SharesDto
            {
                Id = share.Id,
                PostId = share.PostId,
                ShareDateTime = share.ShareDateTime,
                Sharer = new SharerDto
                {
                    Username = share.User?.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}"),
                    UserId = share.UserId
                },
                OriginalPost = new OriginalPostDto
                {
                    Id = share.Post.Id,
                    Description = share.Post.Description,
                    ImageUrl = Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    Username = share.Post.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}"),
                    UserId = share.Post.UserId
                }
            });

            return Ok(sharesDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving all shares: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving all shares.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShareByIdAsync(int id)
    {
        try
        {
            var share = await _shareService.GetShareByIdAsync(id);
            if (share == null) return NotFound();

            var sharesDto = new SharesDto
            {
                Id = share.Id,
                PostId = share.PostId,
                ShareDateTime = share.ShareDateTime,
                Sharer = new SharerDto
                {
                    Username = share.User?.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}"),
                    UserId = share.UserId
                },
                OriginalPost = new OriginalPostDto
                {
                    Id = share.Post.Id,
                    Description = share.Post.Description,
                    ImageUrl = string.IsNullOrEmpty(share.Post.ImageUrl)
                            ? null
                            : Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    Username = share.Post.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}"),
                    UserId = share.Post.UserId
                }
            };

            return Ok(sharesDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user shares: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the user's shares.");
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetSharesByUser()
    {
        try
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var shares = await _shareService.GetSharesByUserAsync(userIdFromToken);

            var sharesDtos = shares.Select(share => new SharesDto
            {
                Id = share.Id,
                PostId = share.PostId,
                ShareDateTime = share.ShareDateTime,
                Sharer = new SharerDto
                {
                    Username = share.User?.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}"),
                    UserId = share.UserId
                },
                OriginalPost = new OriginalPostDto
                {
                    Id = share.Post.Id,
                    Description = share.Post.Description,
                    ImageUrl = Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    Username = share.Post.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}"),
                    UserId = share.Post.UserId
                }
            });

            return Ok(sharesDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user shares: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the user's shares.");
        }
    }

    [HttpGet("getSharesByUsername/{username}")]
    public async Task<IActionResult> GetSharesByUsernameAsync(string username)
    {
        try
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var shares = await _shareService.GetSharesByUserAsync(user.Id);

            var sharesDtos = shares.Select(share => new SharesDto
            {
                Id = share.Id,
                PostId = share.PostId,
                ShareDateTime = share.ShareDateTime,
                Sharer = new SharerDto
                {
                    Username = share.User?.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}"),
                    UserId = share.UserId
                },
                OriginalPost = new OriginalPostDto
                {
                    Id = share.Post.Id,
                    Description = share.Post.Description,
                    ImageUrl = Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    Username = share.Post.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}"),
                    UserId = share.Post.UserId
                }
            });

            return Ok(sharesDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving shares by username: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the shares.");
        }
    }
}
