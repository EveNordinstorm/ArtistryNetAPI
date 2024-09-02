using Microsoft.AspNetCore.Identity;

namespace ArtistryNetAPI.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePhoto { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Share> Shares { get; set; } = new List<Share>();
        public ICollection<Save> Saves { get; set; } = new List<Save>();
    }
}
