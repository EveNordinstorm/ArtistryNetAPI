using ArtistryNetAPI.Entities;

namespace ArtistryNetAPI.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task CreateProductAsync(Product product, IFormFile image, string userId);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
