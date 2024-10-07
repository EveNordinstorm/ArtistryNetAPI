using ArtistryNetAPI.Data;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;

namespace ArtistryNetAPI.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PostService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetPostsByUserNameAsync(string username)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.User.UserName == username)
                .ToListAsync();
        }

        public async Task CreatePostAsync(Post post, IFormFile image, string userId)
        {
            if (image != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images/posts");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                post.ImageUrl = uniqueFileName;
            }

            post.UserId = userId;

            try
            {
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving post: {ex.Message}");
                throw;
            }
        }

        public async Task UpdatePostAsync(Post post)
        {
            try
            {
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating post: {ex.Message}");
                throw;
            }
        }

        public async Task<bool?> DeletePostAsync(int postId, string userId)
        {
            var post = await _context.Posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.Saves)
                .Include(p => p.Shares)
                .SingleOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return null; // Post not found
            }

            if (post.UserId != userId)
            {
                return false; // Not authorized
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return true; // Successfully deleted
        }

    }
}
