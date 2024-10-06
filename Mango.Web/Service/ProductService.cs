using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        private readonly ITokenProvider _tokenProvider;

        public ProductService(IBaseService baseService,
                             ITokenProvider tokenProvider)
        {
            _baseService = baseService;
            _tokenProvider = tokenProvider;
        }

        public Task<ResponseDto> CreateNewProduct(ProductDto productDto)
        {
            var requestDto = new RequestDto
            {
                ApiType = Enums.ApiType.POST,
                Url = $"{Enums.ProductApi}/api/product/Create",
                Data = productDto,
                AccessToken = _tokenProvider.GetToken()
            };

            return _baseService.SendAsync(requestDto);
        }

        public Task<ResponseDto> DeleteProduct(int productId)
        {
            var requestDto = new RequestDto
            {
                ApiType = Enums.ApiType.DELETE,
                Url =$"{Enums.ProductApi}/api/product/Delete/Product/${productId}",
                AccessToken = _tokenProvider.GetToken()
            };

            return _baseService.SendAsync(requestDto);

        }

        public Task<ResponseDto> GetProduct(int productId)
        {
            var requestDto = new RequestDto
            {
                ApiType = Enums.ApiType.GET,
                Url = $"{Enums.ProductApi}/api/product/GetProduct/{productId}",
                AccessToken = _tokenProvider.GetToken()
            };
            return _baseService.SendAsync(requestDto);
        }

        public Task<ResponseDto> GetProducts()
        {
            var requestDto = new RequestDto
            {
                ApiType = Enums.ApiType.GET,
                Url = $"{Enums.ProductApi}/api/product/Products",
                AccessToken = _tokenProvider.GetToken()
            };
            return _baseService.SendAsync(requestDto);
        }

        public Task<ResponseDto> UpdateProduct(ProductDto productDto)
        {
            var requestDto = new RequestDto
            {
                ApiType = Enums.ApiType.POST,
                Url = $"{Enums.ProductApi}/api/product/Update/Product",
                Data = productDto,
                AccessToken = _tokenProvider.GetToken()
            };
            return _baseService.SendAsync(requestDto);
        }
    }
}
