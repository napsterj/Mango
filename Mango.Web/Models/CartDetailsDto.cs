﻿namespace Mango.Web.Models
{
	public class CartDetailsDto
	{
		public int CartDetailsId { get; set; }				
		public int CartHeaderId { get; set; }

		public CartHeaderDto? CartHeaderDto { get; set; }

		public int ProductId { get; set; }
		public ProductDto? ProductDto { get; set; }

		public int Count { get; set; }
	}
}
