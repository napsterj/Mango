using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service
{
	public class CartService : ICartService
	{
		private readonly IBaseService _baseService;
		private readonly ITokenProvider _tokenProvider;

		public CartService(IBaseService baseService, 
						   ITokenProvider tokenProvider)
		{
			_baseService = baseService;
			_tokenProvider = tokenProvider;
		}

		public Task<ResponseDto> ApplyCoupon(CartDto cartDto)
		{
			RequestDto requestDto = new()
			{
				ApiType = Enums.ApiType.POST,
				AccessToken = _tokenProvider.GetToken(),
				Data = cartDto,
				Url = $"{Enums.CartAPIBase}/api/cart/ApplyCoupon"
			};

			return _baseService.SendAsync(requestDto);
		}

		public Task<ResponseDto> LoadCart(string userId)
		{
			RequestDto requestDto = new()
			{
				ApiType = Enums.ApiType.POST,
				AccessToken = _tokenProvider.GetToken(),
				Data = userId,
				Url = $"{Enums.CartAPIBase}/api/cart/load"
			};

			return _baseService.SendAsync(requestDto);
		}

		public Task<ResponseDto> RemoveCoupon(CartDto cartDto)
		{
			RequestDto requestDto = new()
			{
				ApiType = Enums.ApiType.POST,
				AccessToken = _tokenProvider.GetToken(),
				Data = cartDto,
				Url = $"{Enums.CartAPIBase}/api/cart/RemoveCoupon"
			};

			return _baseService.SendAsync(requestDto);
		}

		public Task<ResponseDto> UpsertCart(CartDto cartDto)
		{
			RequestDto requestDto = new()
			{
				ApiType = Enums.ApiType.POST,
				AccessToken = _tokenProvider.GetToken(),
				Data = cartDto,
				Url = $"{Enums.CartAPIBase}/api/cart/CartUpsert"
			};

			return _baseService.SendAsync(requestDto);
		}

        public Task<ResponseDto> EmailCart(CartDto cartDto)
        {
            RequestDto requestDto = new()
            {
                ApiType = Enums.ApiType.POST,
                AccessToken = _tokenProvider.GetToken(),
                Data = cartDto,
                Url = $"{Enums.CartAPIBase}/api/cart/SendCartByEmail"
            };

            return _baseService.SendAsync(requestDto);
        }
    }
}
