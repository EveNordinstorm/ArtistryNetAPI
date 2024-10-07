using System;
using System.ComponentModel.DataAnnotations;

namespace ArtistryNetAPI.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string ProfilePhoto { get; set; } = string.Empty;

        public DateTime PostDateTime { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Share> Shares { get; set; } = new List<Share>();
        public ICollection<Save> Saves { get; set; } = new List<Save>();
    }
}
