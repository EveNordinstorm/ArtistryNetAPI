using Microsoft.AspNetCore.Http;

namespace ArtistryNetAPI.Utilities
{
    public static class FilePathHelper
    {
        public static string FormatImagePath(string path, HttpContext context)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var relativePath = Path.GetRelativePath(context.Request.PathBase.Value, path).Replace("\\", "/");

            var baseUrl = context.Request.Scheme + "://" + context.Request.Host.Value;
            return $"{baseUrl}/images/{relativePath}";
        }
    }
}
