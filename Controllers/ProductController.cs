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

            var product = new Product
            {
                Title = model.Title,
                Price = model.Price,
                UserId = userIdFromToken,
                Username = user.UserName,
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating product: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, "An error occurred while creating the product.");
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();

            var productDtos = products.Select(product => new ProductDto
            {
                Id = product.Id,
                Username = product.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(product.User?.ProfilePhoto)}"),
                Title = product.Title,
                Price = product.Price,
                ImageUrl = Url.Content($"~/images/products/{product.ImageUrl}"),
                UserId = product.UserId
            });

            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving products: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the products.");
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserProducts()
    {
        try
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
                Username = product.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(product.User?.ProfilePhoto)}"),
                Title = product.Title,
                Price = product.Price,
                ImageUrl = Url.Content($"~/images/products/{product.ImageUrl}"),
                UserId = product.UserId
            });

            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user products: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, "An error occurred while retrieving the user's products.");
        }
    }

    [HttpGet("getProductsByUsername/{username}")]
    public async Task<IActionResult> GetProductsByUsernameAsync(string username)
    {
        try
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
                Username = product.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(product.User?.ProfilePhoto)}"),
                Title = product.Title,
                Price = product.Price,
                ImageUrl = Url.Content($"~/images/products/{product.ImageUrl}"),
                UserId = product.UserId
            });

            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving products by username: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the products.");
        }
    }
}                                                                                                 