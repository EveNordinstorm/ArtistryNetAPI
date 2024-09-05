using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Utilities;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Services;

[Route("api/[controller]")]
[ApiController]
public class SharesController : ControllerBase
{
    private readonly IShareService _shareService;

    public SharesController(IShareService shareService)
    {
        _shareService = shareService;
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
                ShareDateTime = DateTime.UtcNow
            };

            await _shareService.AddShareAsync(share);

            return Ok(new { message = "Share added successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding share: {ex.Message}");
            return StatusCode(500, "An error occurred while adding the share.");
        }
    }

    [HttpGet("{postId}")]
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

            var shareDtos = shares.Select(share => new
            {
                share.Id,
                share.PostId,
                share.ShareDateTime,
                Sharer = new
                {
                    Username = share.User?.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}")
                },
                OriginalPost = new
                {
                    share.Post.Id,
                    share.Post.Description,
                    ImageUrl = Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    Username = share.Post.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}")
                }
            });

            return Ok(shareDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user shares: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the user's shares.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShares()
    {
        try
        {
            var shares = await _shareService.GetAllSharesAsync();

            var shareDtos = shares.Select(share => new
            {
                share.Id,
                share.PostId,
                share.ShareDateTime,
                Sharer = new
                {
                    Username = share.User?.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}")
                },
                OriginalPost = new
                {
                    share.Post.Id,
                    share.Post.Description,
                    ImageUrl = Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    Username = share.Post.Username,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}")
                }
            });

            return Ok(shareDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving all shares: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving all shares.");
        }
    }
}
