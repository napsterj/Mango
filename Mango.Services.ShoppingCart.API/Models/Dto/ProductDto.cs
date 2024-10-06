﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCart.API.Models.Dto
{
	public class ProductDto
	{		
		public int ProductId { get; set; }

		[Required]
		public string Name { get; set; }
        
		[DataType(DataType.Currency)]
        [Range(1, 1000)]
		public decimal? ProductPrice { get; set; }

        public int? Count { get; set; }

        public string Description { get; set; }
		public string CategoryName { get; set; }
		public string? ImageUrl { get; set; }
	}
}
