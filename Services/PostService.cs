using ArtistryNetAPI.Data;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ArtistryNetAPI.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PostService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _context.Posts.ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await _context.Posts.FindAsync(id);
        }

        public async Task CreatePostAsync(Post post, IFormFile image)
        {
            // Handle file upload
            string postImagePath = string.Empty;
            if (image != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images/posts");
                Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName; // Use a unique name
                postImagePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(postImagePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Set the image path to the Post object (relative path)
                post.ImagePath = Path.Combine("images/posts", uniqueFileName);
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}
