//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCart.API.Models.Dto
{
    public class CartDetailsDto
    {
        //[Key]		
		public int CartDetailsId { get; set; }
		public int CartHeaderId { get; set; }

        //[ForeignKey("CartHeaderId")]
        public CartHeaderDto?  CartHeaderDto { get; set; }
		public int ProductId { get; set; }

        //[NotMapped]
        public ProductDto? ProductDto { get; set; }
		public int Count { get; set; }
	}
}
