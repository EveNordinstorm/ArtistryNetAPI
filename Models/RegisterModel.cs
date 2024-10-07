using Microsoft.AspNetCore.Http;

namespace ArtistryNetAPI.Models
{
    public class RegisterModel
    {
        public IFormFile ProfilePhoto { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Bio { get; set; }
        public IFormFile? BannerPhoto { get; set; }
    }
}
