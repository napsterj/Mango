using AutoMapper;
using Mango.Services.ShoppingCart.API.Models.Dto;
using Mango.Services.ShoppingCart.API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCart.API.Controllers
{
	[Route("api/cart")]
	[ApiController]
	public class CartAPIController : ControllerBase
	{
		private readonly ICartService _cartService;
		
		ResponseDto _response;

		public CartAPIController(ICartService cartService)
		{
			_response = new ResponseDto();
			_cartService = cartService;
		
		}


		//Three scenarios in Shopping Cart.
		
		//1. The cart is empty : User add first item to the cart. We need to create cart header and cart details
		
		//2. User adds a new item in the cart and the cart has existing items present which were added earlier.
		//   Find cart header with the relevant header id and add cart details under same cart header id 

		//3. User updates only the quantity of an existing item in the cart. Find cart details and update count of
		//the item in cart details.

		//All above three scenarios will be handled by only one action method - CartUpsert
		
		[HttpPost("CartUpsert")]
		[Authorize]
		public async Task<IActionResult> CartUpsert([FromBody]CartDto cartDto)
		{
			_response = await _cartService.UpsertCart(cartDto);

			if(_response != null && !_response.IsSuccess)
			{
				return BadRequest(_response.Message);
			}

			return Ok(_response);
		}

		[HttpPost("Cart/Load")]
		public async Task<IActionResult> LoadCart(string userId)
		{
		    _response = await _cartService.LoadCart(userId);

			if (_response != null && _response.IsSuccess)
			{
				return Ok(_response);
			}

			return BadRequest(_response);
		}

		[HttpPost]
		public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
		{
			_response = await _cartService.ApplyCoupon(cartDto);
			
			if(_response != null && _response.IsSuccess)
			{
				return Ok(_response);
			}
			
			return BadRequest(_response);
		}

		[HttpPost]
		public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
		{
			_response = await _cartService.RemoveCoupon(cartDto);

			if (_response != null && _response.IsSuccess)
			{
				return Ok(_response);
			}

			return BadRequest(_response);
		}
	}
}
