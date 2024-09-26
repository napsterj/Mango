using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI.Services.IServices
{
    public interface IProductService
    {
        Task<ResponseDto> GetProducts();
        Task<ResponseDto> GetProduct(int productId);
        Task<ResponseDto> CreateNewProduct(ProductDto productDto);
        Task<ResponseDto> UpdateProduct(ProductDto productDto);
        Task<bool> DeleteProduct(int productId);
    }
}
