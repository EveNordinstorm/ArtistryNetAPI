using Microsoft.AspNetCore.Http;

namespace ArtistryNetAPI.Models
{
    public class RegisterModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public IFormFile ProfilePhoto { get; set; }
        public string Bio { get; set; } = string.Empty;
    }
}
