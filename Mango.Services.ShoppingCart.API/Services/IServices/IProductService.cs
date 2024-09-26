﻿using Mango.Services.ShoppingCart.API.Models.Dto;

namespace Mango.Services.ShoppingCart.API.Services.IServices
{
	public interface IProductService
	{
		Task<ProductDto> GetProduct(int productId);
	}
}
