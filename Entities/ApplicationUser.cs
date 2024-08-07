using Microsoft.AspNetCore.Identity;

namespace ArtistryNetAPI.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePhoto { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
