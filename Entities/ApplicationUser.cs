using Microsoft.AspNetCore.Identity;

namespace ArtistryNetAPI.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string ProfilePhoto { get; set; } = string.Empty;
        public string? BannerPhoto { get; set; }
        public string Bio { get; set; } = string.Empty;

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Share> Shares { get; set; } = new List<Share>();
        public ICollection<Save> Saves { get; set; } = new List<Save>();
        public ICollection<Follower> Following { get; set; } = new List<Follower>();
        public ICollection<Follower> Followers { get; set; } = new List<Follower>();
    }
}
