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
using ArtistryNetAPI.Services;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ApplicationDbContext _context;

    public ProductsController(IProductService productService, ApplicationDbContext context)
    {
        _productService = productService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromForm] ProductModel model, [FromForm] IFormFile imageUrl)
    {
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

            var product = new Product
            {
                Title = model.Title,
                Price = model.Price,
                UserId = userIdFromToken,
                UserName = user.UserName,
                ProfilePhoto = user.ProfilePhoto
            };

            await _productService.CreateProductAsync(product, imageUrl, userIdFromToken);

            var imageUrlResult = Url.Content($"~/images/products/{product.ImageUrl}");

            return Ok(new
            {
                Message = "Product created successfully",
                ImageUrl = imageUrlResult
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);
            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            if (product.UserId != userIdFromToken)
            {
                return Forbid("You are not authorized to delete this product.");
            }

            await _productService.DeleteProductAsync(id);

            return Ok(new { message = "Product deleted successfully" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        {
            var products = await _productService.GetAllProductsAsync();

            var productDtos = products.Select(product => new ProductDto
            {
                Id = product.Id,
                UserName = product.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(product.User?.ProfilePhoto)}"),
                Title = product.Title,
                Price = product.Price,
                ImageUrl = Url.Content($"~/images/products/{product.ImageUrl}"),
                UserId = product.UserId
            });

            return Ok(productDtos);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductByIdAsync(int id)
    {
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            var productDto = new ProductDto
            {
                Id = product.Id,
                UserName = product.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(product.User?.ProfilePhoto)}"),
                Title = product.Title,
                Price = product.Price,
                ImageUrl = Url.Content($"~/images/products/{product.ImageUrl}"),
                UserId = product.UserId
            };

            return Ok(productDto);
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserProducts()
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var products = await _context.Products
                .Include(p => p.User)
                .Where(p => p.UserId == userIdFromToken)
                .ToListAsync();

            var productDtos = products.Select(product => new ProductDto
            {
                Id = product.Id,
                UserName = product.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(product.User?.ProfilePhoto)}"),
                Title = product.Title,
                Price = product.Price,
                ImageUrl = Url.Content($"~/images/products/{product.ImageUrl}"),
                UserId = product.UserId
            });

            return Ok(productDtos);
        }
    }

    [HttpGet("getProductsByUserName/{username}")]
    public async Task<IActionResult> GetProductsByUserNameAsync(string username)
    {
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var products = await _context.Products
                .Include(p => p.User)
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            var productDtos = products.Select(product => new ProductDto
            {
                Id = product.Id,
                UserName = product.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(product.User?.ProfilePhoto)}"),
                Title = product.Title,
                Price = product.Price,
                ImageUrl = Url.Content($"~/images/products/{product.ImageUrl}"),
                UserId = product.UserId
            });

            return Ok(productDtos);
        }
    }
}                                                                                                 