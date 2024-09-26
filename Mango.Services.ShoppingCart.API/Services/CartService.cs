using AutoMapper;
using Mango.Services.ShoppingCart.API.Data;
using Mango.Services.ShoppingCart.API.Models;
using Mango.Services.ShoppingCart.API.Models.Dto;
using Mango.Services.ShoppingCart.API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCart.API.Services
{
	public class CartService : ICartService
	{
		private readonly CartDbContext _cartContext;
		private readonly IMapper _mapper;
		private readonly IProductService _productService;
		private readonly ICouponService _couponService;
		private ResponseDto _responseDto;

		public CartService(CartDbContext cartContext,
						   IMapper mapper,
						   IProductService productService,
						   ICouponService couponService)
		{
			_cartContext = cartContext;
			_mapper = mapper;
			_productService = productService;
			_couponService = couponService;
			_responseDto = new ResponseDto();
		}

		public async Task<ResponseDto> ApplyCoupon(CartDto cartDto)
		{
			try
			{
				var cartHeader = await _cartContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
			
				if (cartHeader != null)
				{
					cartHeader.CouponCode = cartDto.CartHeader.CouponCode;
					_cartContext.CartHeaders.Update(cartHeader);
					await _cartContext.SaveChangesAsync();
					_responseDto.IsSuccess = true;					
				}								
			}
			catch (Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Result = ex.Message;
			}

			return _responseDto;
		}		

		public async Task<ResponseDto> LoadCart(string userId)
		{			
			var cardDto = new CartDto();
		
			try
			{
				// get cart header 
				cardDto.CartHeader = _mapper.Map<CartHeaderDto>(await _cartContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId));

				//get cart details
				cardDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_cartContext.CartDetails.Where(u => u.CartHeaderId == cardDto.CartHeader.CartHeaderId));

				// get each product added from card details and extract name of product, price from product table
				foreach (var item in cardDto.CartDetails)
				{
					item.Product = _mapper.Map<ProductDto>(await _productService.GetProduct(item.ProductId));
					item.CartHeaderDto.CartTotal += item.Count * item.Product.ProductPrice;
				}

				if (!string.IsNullOrWhiteSpace(cardDto.CartHeader.CouponCode))
				{
					var couponDto = await _couponService.GetCouponByCode(cardDto.CartHeader.CouponCode);
					if (cardDto.CartHeader.CartTotal > couponDto.MinAmount)
					{
						cardDto.CartHeader.CartTotal -= couponDto.DiscountAmount;
						cardDto.CartHeader.Discount = couponDto.DiscountAmount;
					}
				}

				_responseDto.IsSuccess = true;
				_responseDto.Result = cardDto;
				
				return _responseDto;								
			}
			catch(Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = ex.Message;
			}

			return _responseDto;
		}

		public async Task<ResponseDto> RemoveCoupon(CartDto cartDto)
		{
			try
			{
				var cartHeader = await _cartContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
				if (cartHeader != null)
				{
					cartHeader.CouponCode = "";
					_cartContext.CartHeaders.Update(cartHeader);
					await _cartContext.SaveChangesAsync();
					_responseDto.IsSuccess = true;
				}
			}
			catch(Exception ex) 
			{
				_responseDto.IsSuccess = false;
				_responseDto.Result = ex.Message;
			}

			return _responseDto;
		}

		public async Task<ResponseDto> UpsertCart(CartDto cartDto)
		{
			try
			{
				//retrieve cart header
				var cartHeader = await _cartContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == cartDto.CartHeader.UserId);
				if (cartHeader == null)
				{
					var cardHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);

					await _cartContext.CartHeaders.AddAsync(cardHeader);
					await _cartContext.SaveChangesAsync();

					var cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails);

					cartDto.CartDetails.First().Product = await _productService.GetProduct(cartDto.CartDetails.First().ProductId);
					cartDto.CartDetails.First().CartHeaderId = cardHeader.CartHeaderId;

					cartDetails.CartHeaderId = cardHeader.CartHeaderId;

					await _cartContext.CartDetails.AddAsync(cartDetails);
					await _cartContext.SaveChangesAsync();
				}
				else
				{
					var existingUserCartDetails = await _cartContext.CartDetails
														.FirstOrDefaultAsync(cd => cd.ProductId == cartDto.CartDetails.First().ProductId &&
																			 cd.CartHeader.CartHeaderId == cartDto.CartHeader.CartHeaderId);

					if (existingUserCartDetails == null)
					{
						cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
						var cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails);
						await _cartContext.CartDetails.AddAsync(cartDetails);
						await _cartContext.SaveChangesAsync();

					}
					else
					{
						cartDto.CartDetails.First().Count += existingUserCartDetails.Count;
						cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
					    cartDto.CartDetails.First().CartDetailsId =	existingUserCartDetails.CartDetailsId;
						
						_cartContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails));
						await _cartContext.SaveChangesAsync();
					}
				}

				_responseDto.IsSuccess = true;
				_responseDto.Result = cartDto;
				return _responseDto;
			}
			catch(Exception ex)
			{
				_responseDto.IsSuccess=false;
				_responseDto.Message = ex.Message;
			}

			return _responseDto;
		}
	}
}
