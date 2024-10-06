using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.Email.API.Models.Dto
{
	public class CartHeaderDto
	{
		[Key]
		public int CartHeaderId { get; set; }
		public string? UserId { get; set; }
		public string? CouponCode { get; set; }

		[NotMapped]
		public decimal Discount { get; set; }
		[NotMapped]
		public decimal CartTotal { get; set; }
		public string Email {  get; set; }
	}
}
