﻿namespace ArtistryNetAPI.Dto
{
    public class RegisterDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public IFormFile ProfilePhoto { get; set; }
        public string Bio { get; set; } = string.Empty;
    }
}
