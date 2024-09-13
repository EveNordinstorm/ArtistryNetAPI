﻿namespace ArtistryNetAPI.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string ProfilePhoto { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public int Price { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}