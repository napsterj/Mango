﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCart.API.Models
{
	public class CartHeader
	{
		[Key]
		public int CartHeaderId { get; set; }
		public string? UserId { get; set; }
		public string? CouponCode { get; set; }

		[NotMapped]
		public decimal Discount { get; set; }
		[NotMapped]
		public decimal CartTotal { get; set; }
	}
}
