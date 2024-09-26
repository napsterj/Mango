using Mango.Services.ShoppingCart.API.Models.Dto;

namespace Mango.Services.ShoppingCart.API.Services.IServices
{
	public interface ICartService
	{
		Task<ResponseDto> UpsertCart(CartDto cartDto);
		Task<ResponseDto> LoadCart(string userId);
		Task<ResponseDto> ApplyCoupon(CartDto cartDto);
		Task<ResponseDto> RemoveCoupon(CartDto cartDto);
	}
}
