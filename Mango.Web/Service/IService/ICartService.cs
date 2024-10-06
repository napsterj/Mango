using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
	public interface ICartService
	{
		Task<ResponseDto> LoadCart(string userId);
		Task<ResponseDto> UpsertCart(CartDto cartDto);
		Task<ResponseDto> ApplyCoupon(CartDto cartDto);
		Task<ResponseDto> RemoveCoupon(CartDto cartDto);
		Task<ResponseDto> EmailCart(CartDto cartDto);

    }
}
