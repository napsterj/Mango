namespace Mango.Services.ShoppingCart.API.Models.Dto
{
	public class CartDto
	{
		public CartHeaderDto? CartHeaderDto { get; set; }
		public IEnumerable<CartDetailsDto>? CartDetailsDto { get; set; }

	}
}
