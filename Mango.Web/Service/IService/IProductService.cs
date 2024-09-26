using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDto> GetProducts();
        Task<ResponseDto> GetProduct(int productId);
        Task<ResponseDto> UpdateProduct(ProductDto productDto);
        Task<ResponseDto> DeleteProduct(int productId);
        Task<ResponseDto> CreateNewProduct(ProductDto productDto);
    }
}
