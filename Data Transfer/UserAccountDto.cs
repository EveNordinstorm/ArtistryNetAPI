﻿namespace ArtistryNetAPI.Dto
{
    public class UserAccountDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfilePhoto { get; set; }
        public string? BannerPhoto { get; set; }
        public string Bio { get; set; }
    }
}
