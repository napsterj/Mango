﻿using Mango.Services.ShoppingCart.API.Models.Dto;

namespace Mango.Services.ShoppingCart.API.Services.IServices
{
	public interface ICouponService
	{
		Task<CouponDto> GetCouponByCode(string couponCode);		
	}
}
